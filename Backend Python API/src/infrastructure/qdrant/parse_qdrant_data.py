from .embedding_service import EmbeddingService
from ...domain.extract_text_schema import FileSchema
from ...domain.qdrant_insert_schema import QdrantInsertSchema, QdrantMetadata

embedding_service = EmbeddingService()

def parse_qdrant_data(data: FileSchema, id_agent: str, id_file: str) -> list[dict]:
    """Converte um FileSchema em uma lista de dicionários prontos para inserção no Qdrant."""
    parsed_data = []

    for page in data.pages:
        # Garante que page.text seja sempre uma lista
        texts = page.text if isinstance(page.text, list) else [page.text]

        for text in filter(str.strip, texts):  # ignora textos vazios automaticamente
            embedding = embedding_service.generate_embedding(text)

            metadata = QdrantMetadata(
                id_agent=id_agent,
                id_file=id_file,
                type=data.file_type,
                file_name=data.file_name,
                page_number=page.page_number,
            )

            parsed_data.append(
                QdrantInsertSchema(
                    vector=embedding,
                    text=text,
                    metadata=metadata,
                ).dict()
            )

    return parsed_data
