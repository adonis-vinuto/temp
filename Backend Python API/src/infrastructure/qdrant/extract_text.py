import io
from pathlib import Path
from typing import List, Dict, Any
import PyPDF2
import docx
import pandas as pd

def extract_from_file(content: bytes, filename: str) -> List[Dict[str, Any]]:
    """
    Extrai texto de arquivos e retorna lista de páginas com texto bruto
    
    Returns:
        List[Dict]: [{"text": "conteudo", "page_number": 1}, ...]
    """
    file_extension = Path(filename).suffix.lower()
    
    if file_extension == '.pdf':
        return _extract_pdf(content)

    elif file_extension == '.docx':
        return _extract_docx(content)
    
    elif file_extension == '.txt':
        return _extract_txt(content)
    
    elif file_extension == '.xlsx':
        return _extract_excel(content)
    else:
        raise ValueError(f"Formato não suportado: {file_extension}")

def _extract_pdf(content: bytes) -> List[Dict[str, Any]]:
    """Extrai texto de PDF página por página"""
    file_stream = io.BytesIO(content)

    try:
        pdf_reader = PyPDF2.PdfReader(file_stream)

        pages = []

        for page_num, page in enumerate(pdf_reader.pages, 1):
            text = page.extract_text().strip()
            pages.append({"text": text, "page_number": page_num})

        return pages
    
    finally:
        file_stream.close()

def _extract_docx(content: bytes) -> List[Dict[str, Any]]:
    """Extrai texto de DOCX, separando páginas por quebras de página"""
    file_stream = io.BytesIO(content)

    try:
        doc = docx.Document(file_stream)
        pages = []
        page_num = 1
        page_text = []

        for para in doc.paragraphs:
            text = para.text.strip()
            if text:
                page_text.append(text)

            # Detecta quebra de página manual
            if para._element.xpath('.//w:br[@w:type="page"]'):
                if page_text:
                    pages.append({"text": "\n".join(page_text), "page_number": page_num})
                    page_num += 1
                    page_text = []

        # Adiciona o restante como última página
        if page_text:
            pages.append({"text": "\n".join(page_text), "page_number": page_num})

        return pages

    finally:
        file_stream.close()

def _extract_txt(content: bytes) -> List[Dict[str, Any]]:
    """Extrai texto de TXT"""
    content_text = content.decode('utf-8').strip()

    return [{"text": content_text, "page_number": 1}]

def _extract_excel(content: bytes) -> List[Dict[str, Any]]:
    """Extrai tabelas de Excel e retorna cada uma em markdown"""
    file_stream = io.BytesIO(content)
    
    try:
        # Lê todas as planilhas
        xls = pd.ExcelFile(file_stream)

        tables = []

        for idx, sheet_name in enumerate(xls.sheet_names, 1):
            df = pd.read_excel(xls, sheet_name=sheet_name)

            # Substitui valores nulos antes de converter
            df = df.fillna(0)  # ou .fillna("sem valor")

            md_table = df.to_markdown(index=False)

            tables.append({"text": md_table, "page_number": idx})

        return tables
    
    finally:
        file_stream.close()
