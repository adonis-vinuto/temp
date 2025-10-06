from typing import List, Dict
from uuid import uuid4
from ...domain.qdrant_insert_schema import QdrantInsertSchema, QdrantMetadata
from .embedding_service import EmbeddingService


def chunk_and_prepare_qdrant(
    pages: List[Dict],
    id_agent: str,
    id_file: str,
    file_type: str,
    file_name: str
) -> List[QdrantInsertSchema]:
    """
    Recebe páginas de texto e gera objetos prontos para Qdrant.
    Cada página é um chunk completo (sem divisão por tamanho).
    
    :param pages: Lista de dicionários {"text": str, "page_number": int}
    :param id_agent: ID do agente
    :param id_file: ID do arquivo
    :param file_type: Tipo do arquivo
    :param file_name: Nome do arquivo
    :return: Lista de QdrantInsertSchema
    """

    embedding_service = EmbeddingService()
    
    # 1️⃣ Filtra páginas vazias
    valid_pages = [p for p in pages if p.get("text") and str(p.get("text")).strip()]
    
    # 2️⃣ Gera embeddings (uma por página)
    texts = [p["text"] for p in valid_pages]
    embeddings = embedding_service.generate_embeddings(texts)
    
    # 3️⃣ Cria objetos QdrantInsertSchema
    qdrant_docs = []
    for i, page in enumerate(valid_pages):
        metadata = QdrantMetadata(
            id_agent=id_agent,
            id_file=id_file,
            type=file_type,
            file_name=file_name,
            page_number=page["page_number"]
        )
        qdrant_doc = QdrantInsertSchema(
            id=str(uuid4()),
            vector=embeddings[i],
            text=page["text"],
            metadata=metadata
        )
        qdrant_docs.append(qdrant_doc)
    
    return qdrant_docs

# --- Exemplo de uso ---
# pages = [{"text": "conteúdo da tabela 1...", "page_number": 1}, ...]
# qdrant_chunks = chunk_and_prepare_qdrant(pages, id_agent, id_file, "pdf", "arquivo.pdf", embedding_service, 500)
