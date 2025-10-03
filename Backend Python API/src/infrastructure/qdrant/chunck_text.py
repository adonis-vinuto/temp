from typing import List, Dict, Any

"""Responsável apenas pela divisão de texto em chunks"""

def chunk_pages(pages: List[Dict[str, Any]], max_chunk_size: int) -> List[Dict[str, Any]]:
    """
    Divide páginas em chunks menores se necessário
    """
    chunked_pages = []

    for page in pages:
        text = page["text"]
        page_num = page["page_number"]

        if len(text) <= max_chunk_size:
            chunked_pages.append({
                "text": text,
                "page_number": page_num
            })

        else:
            chunks = _split_text(text, max_chunk_size)

            for chunk in chunks:
                chunked_pages.append({
                    "text": chunk,
                    "page_number": page_num
                })

    return chunked_pages

def _split_text(text: str, max_size: int) -> List[str]:
    """Divide texto em chunks baseado em palavras"""
    words = text.split()
    chunks = []
    current_chunk = ""

    for word in words:
        if len(current_chunk) + len(word) + 1 <= max_size:
            current_chunk = current_chunk + " " + word if current_chunk else word
        else:
            if current_chunk:
                chunks.append(current_chunk)
            current_chunk = word

    if current_chunk:
        chunks.append(current_chunk)

    return chunks