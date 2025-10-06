from fastapi import UploadFile
from pydantic import BaseModel
from typing import List, Optional, Any, Dict

class DocsortRequest:
    """Schema para requisição de extração de dados."""
    def __init__(self, file: UploadFile, fields: List[Dict[str, Any]] = None):
        self.file = file
        self.fields = fields or []

class DocsortResponse(BaseModel):
    """Schema para resposta de extração de dados."""
    extracted_data: Dict[str, Any]
    usage: Optional[dict] = None

class OCRConfig:
    """Configurações predefinidas para diferentes tipos de documento"""
    
    # Configuração para documentos de alta qualidade
    HIGH_QUALITY = {
        'dpi': 300,
        'tesseract_config': '--oem 3 --psm 6'
    }
    
    # Configuração para documentos escaneados/baixa qualidade
    LOW_QUALITY = {
        'dpi': 400,
        'tesseract_config': '--oem 3 --psm 6 -c tessedit_char_whitelist=0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz àáâãéêíóôõúç'
    }
    
    # Configuração para documentos com tabelas
    TABLES = {
        'dpi': 350,
        'tesseract_config': '--oem 3 --psm 6'
    }
    
    # Configuração rápida (menor qualidade, mais velocidade)
    FAST = {
        'dpi': 200,
        'tesseract_config': '--oem 3 --psm 6'
    }

def get_data_extractor_prompt(fields: str) -> str:
    prompt = (
        "Você é um assistente especializado em extrair informações de documentos fiscais brasileiros.\n"
        "Analise o texto fornecido e extraia EXATAMENTE os seguintes campos:\n\n"
        f"{fields}\n\n"
        "INSTRUÇÕES IMPORTANTES:\n"
        "1. Para cada campo solicitado, retorne um par chave-valor onde:\n"
        "   - key: deve ser EXATAMENTE o nome do campo especificado (ex: 'doc-type', 'data-emissao')\n"
        "   - value: o valor encontrado no documento, ou 'NAO_IDENTIFICADO' se não encontrar\n\n"
        "2. Regras de extração:\n"
        "   - Tipo de documento: identifique se é NF-e, NFS-e, NFC-e ou Cupom Fiscal\n"
        "   - Datas: extraia no formato solicitado (ex: MM/YYYY, DD-MM-AA)\n"
        "   - Valores monetários: mantenha o formato original encontrado no documento\n"
        "   - Números de documento:\n"
        " - Para NF-e: número da nota fiscal eletrônica (remova zeros à esquerda)\n"
        " - Para NFS-e: número da nota de serviço eletrônica (remova zeros à esquerda)\n"
        " - Para NFS-e: use o número do RPS Substituto (não o número da NFS-e, remova zeros à esquerda)\n"
        " - Para NFC-e/Cupom: número do cupom fiscal (remova zeros à esquerda)\n\n"
        "   - Nomes/Razão social: extraia do DESTINATÁRIO/RECEPTOR, não do emissor\n\n"
        "3. Mapeamento de tipos:\n"
        "   - NF-e -> 'Nota Fiscal'\n"
        "   - NFS-e -> 'Serviço'\n"
        "   - NFC-e -> 'Cupom'\n"
        "   - Cupom Fiscal -> 'Cupom'\n\n"
        "4. Se um campo não for encontrado ou estiver ilegível, use 'NAO_IDENTIFICADO'\n\n"
        "5. SEMPRE inclua todos os campos solicitados, mesmo que não encontrados\n\n"
        "Texto do documento a ser analisado:\n"
        "\n"
        "FORMATO DE RESPOSTA:\n"
        "Retorne um JSON válido no formato:\n"
        "{\n"
        '    "data": [\n'
        '        {"key": "nome-do-campo", "value": "valor-extraido"},\n'
        '        {"key": "outro-campo", "value": "outro-valor"}\n'
        "    ]\n"
        "}\n\n"
        "Texto do documento:\n"
    )
    return prompt