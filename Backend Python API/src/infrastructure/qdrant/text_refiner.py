from typing import List, Union, Tuple, Dict, Any
from langchain_groq import ChatGroq
from ..configs import config
from ..simple_usage_callback import SimpleUsageCallback
        
llm = ChatGroq(
    api_key=config.GROQ_API_KEY,
    model_name="openai/gpt-oss-20b",
    temperature=0.3
)

def text_resume(pages: Union[List, str], usage_callback: SimpleUsageCallback, tamanho_maximo: int = 1000, limite_texto: int = 4000) -> str:
    """
    Resume um texto limitando a entrada a um número específico de caracteres
    
    Args:
        pages: Páginas do documento (lista de objetos com .text) ou string direta
        usage_callback: Callback compartilhado para rastrear usage
        tamanho_maximo: Tamanho máximo do resumo em caracteres
        limite_texto: Limite máximo de caracteres do texto de entrada
        
    Returns:
        Texto resumido
    """
    raw_text = ""

    # Se receber uma string diretamente
    if isinstance(pages, str):
        raw_text = pages[:limite_texto]
    # Se receber uma lista de objetos/dicts
    else:
        for page in pages:
            # Tenta acessar como objeto com atributo .text
            if hasattr(page, 'text'):
                page_text = page.text
            # Tenta acessar como dicionário
            elif isinstance(page, dict):
                page_text = page.get('text', '')
            else:
                page_text = str(page)
            
            if len(raw_text) + len(page_text) <= limite_texto:
                raw_text += page_text + " "
            else:
                remaining_chars = limite_texto - len(raw_text)
                if remaining_chars > 0:
                    raw_text += page_text[:remaining_chars]
                break

    raw_text = raw_text.strip()
    
    if not raw_text:
        return "Texto vazio ou muito curto para resumir."

    prompt = f"""
    Resuma o seguinte texto de forma clara e concisa em no máximo {tamanho_maximo} caracteres:
    
    {raw_text}
    
    Resumo:
    """
    
    response = llm.invoke(prompt, config={"callbacks": [usage_callback]})
    return response.content.strip()

def text_file_name(pages: Union[List, str], usage_callback: SimpleUsageCallback, tamanho_maximo: int = 100, limite_texto: int = 4000) -> str:
    """
    Gera um nome de arquivo para o texto limitando a entrada a um número específico de caracteres

    Args:
        pages: Páginas do documento (lista de objetos com .text) ou string direta
        usage_callback: Callback compartilhado para rastrear usage
        tamanho_maximo: Tamanho máximo do nome do arquivo em caracteres
        limite_texto: Limite máximo de caracteres do texto de entrada
        
    Returns:
        Nome do arquivo gerado
    """
    raw_text = ""

    # Se receber uma string diretamente
    if isinstance(pages, str):
        raw_text = pages[:limite_texto]
    # Se receber uma lista de objetos/dicts
    else:
        for page in pages:
            # Tenta acessar como objeto com atributo .text
            if hasattr(page, 'text'):
                page_text = page.text
            # Tenta acessar como dicionário
            elif isinstance(page, dict):
                page_text = page.get('text', '')
            else:
                page_text = str(page)
            
            if len(raw_text) + len(page_text) <= limite_texto:
                raw_text += page_text + " "
            else:
                remaining_chars = limite_texto - len(raw_text)
                if remaining_chars > 0:
                    raw_text += page_text[:remaining_chars]
                break

    raw_text = raw_text.strip()
    
    if not raw_text:
        return "Texto vazio ou muito curto para gerar um nome de arquivo."

    prompt = f"""
    Gera um nome de arquivo claro e conciso em no máximo {tamanho_maximo} caracteres, nao coloque nenhum tipo de extensao, apenas o nome, para o seguinte texto:

    {raw_text}

    Nome do arquivo:
    """
    
    response = llm.invoke(prompt, config={"callbacks": [usage_callback]})
    return response.content.strip()
