import os
import re
import json
from dotenv import load_dotenv
from langchain.chat_models import init_chat_model

from ..domain.docsort import DocsortRequest, DocsortResponse, get_data_extractor_prompt
from ..infrastructure.advanced_ocr_processor import  AdvancedOcrProcessor

load_dotenv()

class DocsortService:
    """
    Serviço para extrair dados estruturados de documentos fiscais usando LLM.
    
    Recebe texto de documentos (NF-e, NFS-e, Cupom) e uma lista de campos
    personalizáveis, retornando dados extraídos em formato JSON.
    
    Exemplo:
    ```python
    fields = [{"name_field": "doc-type", "description_field": "Tipo do documento"}]
    response = await service.execute(DataExtractorRequest(text, fields))
    # retorna: {"extracted_data": {"doc-type": "NF-e"}, "usage": {...}}
    ```
    """

    def __init__(self, ocr_processor: AdvancedOcrProcessor):
        self.ocr_processor = ocr_processor
        self.llm_client = init_chat_model(model="openai/gpt-oss-120b", model_provider="groq", temperature=0)

    def _parse_llm_response(self, content: str, fields: list):
        """Parse manual da resposta do LLM para extrair dados estruturados"""
        
        # Inicializar com todos os campos como NAO_IDENTIFICADO
        extracted_data = {field['name_field']: "NAO_IDENTIFICADO" for field in fields}
        
        try:
            # Tentar parsing JSON direto
            parsed_json = json.loads(content)
            
            # Se veio como lista de key-value pairs
            if 'data' in parsed_json and isinstance(parsed_json['data'], list):
                for item in parsed_json['data']:
                    if isinstance(item, dict) and 'key' in item and 'value' in item:
                        key = item['key']
                        value = item['value'] or "NAO_IDENTIFICADO"
                        if key in extracted_data:
                            extracted_data[key] = value
            
            # Se veio como objeto direto
            elif isinstance(parsed_json, dict):
                for key, value in parsed_json.items():
                    if key in extracted_data:
                        extracted_data[key] = value or "NAO_IDENTIFICADO"
                        
        except json.JSONDecodeError:
            # Fallback: tentar extrair com regex
            print("JSON inválido, tentando regex fallback...")
            
            # Padrões para capturar pares chave-valor
            patterns = [
                r'"([^"]+)"\s*:\s*"([^"]*)"',  # "key": "value"
                r"'([^']+)'\s*:\s*'([^']*)'",  # 'key': 'value'
                r'([a-zA-Z][a-zA-Z0-9_-]+)\s*:\s*"([^"]*)"',  # key: "value"
                r'([a-zA-Z][a-zA-Z0-9_-]+)\s*:\s*([^\n,}]+)'  # key: value
            ]
            
            for pattern in patterns:
                matches = re.findall(pattern, content)
                for key, value in matches:
                    key = key.strip()
                    value = value.strip().strip('"\'')
                    if key in extracted_data and value:
                        extracted_data[key] = value
                        
        print(f"Dados extraídos após parsing: {extracted_data}")
        return extracted_data

    async def handle(self, request: DocsortRequest) -> DocsortResponse:
        file = request.file

         # Validação do formato do arquivo
        if not file.filename or not file.filename.lower().endswith('.pdf'):
            raise Exception("Apenas arquivos PDF são aceitos.")
        
        file_content = await file.read()

        try:
            # Extrai as páginas como lista de dicionários
            extracted_pages = self.ocr_processor.extract_text_from_pdf_bytes(file_content)
            
            # Junta todos os textos das páginas
            extracted_text = "\n\n--- PAGE BREAK ---\n\n".join([page['text'] for page in extracted_pages])

            print(f"Páginas extraídas: {len(extracted_pages)}")
            print(f"Texto total: {len(extracted_text)} caracteres")
        except Exception as e:
            raise Exception(f"Erro ao processar o arquivo PDF: {str(e)}")
        
        
        fields = request.fields

        # Validando o texto e a lista de campos
        if not extracted_text or len(extracted_text.strip()) < 5:
            raise Exception({
                "extracted_text": ["Texto muito curto ou vazio para processamento."]
            })
        
        if not fields:
            raise Exception({
                "fields": ["Campos são obrigatórios para processamento."]
            })

        # Preparar lista de campos para o prompt
        fields_description = []
        for field in fields:
            fields_description.append(f"- {field['name_field']}: {field['description_field']}")
        
        fields_text = "\n".join(fields_description)

        # Modificar o prompt para solicitar JSON estruturado
        prompt = get_data_extractor_prompt(fields=fields_text) + f"\n{extracted_text}\n"

        try:
            # Usar LLM normal (sem structured output) para capturar usage
            response = await self.llm_client.ainvoke(prompt)

            print("Resposta bruta do LLM:", response.content)
            
            # Extrair usage se disponível
            usage_info = None
            if hasattr(response, 'response_metadata') and 'token_usage' in response.response_metadata:
                usage_info = response.response_metadata
            elif hasattr(response, 'usage_metadata'):
                usage_info = {"token_usage": response.usage_metadata}

            # Parse manual do conteúdo
            extracted_data = self._parse_llm_response(response.content, fields)
            
            print("Dados extraídos estruturados:", extracted_data)

            return DocsortResponse(
                extracted_data=extracted_data,
                usage=usage_info
            )
            
        except Exception as e:
            print(f"Erro no processamento: {str(e)}")
            # Fallback: retornar todos os campos como NAO_IDENTIFICADO
            fallback_data = {field['name_field']: "NAO_IDENTIFICADO" for field in fields}
            return DocsortResponse(
                extracted_data=fallback_data,
                usage=None
            )