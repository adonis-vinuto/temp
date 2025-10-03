from typing import List
from uuid import uuid4
from ...domain.qdrant_insert_schema import QdrantInsertSchema, QdrantMetadata
from ...domain.extract_text_schema import FileSchema, PageSchema
from .text_refiner import text_resume, text_file_name
from .extract_text import extract_from_file
from .embedding_service import EmbeddingService
from .chunck_text import chunk_pages

class DocumentProcessor:
    def __init__(self, max_chunk_size: int = 1000):
        self.max_chunk_size = max_chunk_size
        self.embedding_service = EmbeddingService()
    
    def _extract_file_type(self, filename: str) -> str:
        if '.' in filename:
            return filename.split('.')[-1].lower()
    
        return "unknown"

    def process_file_for_qdrant(self, file_data: FileSchema, id_agent: str, id_file: str) -> List[QdrantInsertSchema]:
        pages_dict = [
            {"text": page.text, "page_number": page.page_number}
            for page in file_data.pages
        ]
        
        chunked_pages = chunk_pages(pages_dict, self.max_chunk_size)

        # filtra textos vazios (o serviço de embedding também filtra, mas precisamos manter o alinhamento)
        valid_pages = [p for p in chunked_pages if p.get("text") and str(p.get("text")).strip()]

        texts = [page["text"] for page in valid_pages]

        embeddings = self.embedding_service.generate_embeddings(texts)

        qdrant_documents = []

        # iterar apenas sobre as páginas que foram usadas para gerar embeddings
        for i, page in enumerate(valid_pages):

            metadata = QdrantMetadata(
                id_agent=id_agent,
                id_file=id_file,
                type=file_data.file_type,
                file_name=file_data.file_name,
                page_number=page["page_number"]
            )

            qdrant_doc = QdrantInsertSchema(
                id=str(uuid4()),
                vector=embeddings[i],
                text=page["text"],
                metadata=metadata
            )

            qdrant_documents.append(qdrant_doc)

        return qdrant_documents
    
    async def extract_text(self, content: bytes, filename: str):
        """
        Função principal que orquestra extração e chunking
        """
        extracted_pages = extract_from_file(content, filename)

        file_type = self._extract_file_type(filename)

        pages = []
        for page in extracted_pages:
            pages.append(
                PageSchema(
                    text=page["text"],
                    page_number=page["page_number"]
                )
            )

        resume = text_resume(pages, tamanho_maximo=1000)

        filename = text_file_name(pages, tamanho_maximo=100)
        
        file_schema = FileSchema(
            file_name=filename,
            file_type= file_type,
            resume=resume,
            pages=pages
        )

        return file_schema
