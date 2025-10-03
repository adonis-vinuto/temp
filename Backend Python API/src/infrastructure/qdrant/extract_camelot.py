import camelot
import pandas as pd
import tempfile
import os
from typing import List, Dict

def extract_camelot(file_content: bytes, filename: str = "temp.pdf") -> List[Dict]:
    """
    Extrai todas as tabelas de um PDF (bytes) e retorna como lista de dicts.
    
    :param file_content: Conteúdo do arquivo PDF em bytes
    :param filename: Nome do arquivo (opcional)
    :return: Lista de dicts com 'text' e 'page_number'
    """
    # Cria arquivo temporário
    with tempfile.NamedTemporaryFile(delete=False, suffix=".pdf") as tmp_file:
        tmp_file.write(file_content)
        tmp_path = tmp_file.name
    
    try:
        # Extrair todas as tabelas do PDF
        tables = camelot.read_pdf(tmp_path, pages="all", flavor="stream")
        
        pages = []
        
        for idx, table in enumerate(tables, 1):
            df = table.df.copy()
            
            # Converte números: remove separador de milhar e troca vírgula por ponto
            for col in df.columns[1:]:  # ignora a primeira coluna
                if df[col].dtype == 'object':
                    df[col] = (
                        df[col]
                        .astype(str)
                        .str.replace(".", "", regex=False)
                        .str.replace(",", ".", regex=False)
                    )
                    df[col] = pd.to_numeric(df[col], errors="ignore")
            
            # Converte a tabela inteira para texto
            table_text = df.to_string(index=False)
            
            pages.append({
                "text": table_text, 
                "page_number": table.page
            })
        
        return pages
    
    finally:
        # Remove arquivo temporário
        if os.path.exists(tmp_path):
            os.remove(tmp_path)