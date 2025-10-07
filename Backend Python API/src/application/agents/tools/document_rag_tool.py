from langchain_core.tools import tool
from langchain_openai import OpenAIEmbeddings
from src.infrastructure.qdrant.qdrant_service import QdrantService
from src.infrastructure.configs import config
from typing import List, Dict, Any
from src.domain.chat import Document
 
# A inicialização dos serviços pode ficar fora da factory se for compartilhada
# ou dentro, se cada ferramenta precisar de sua própria instância.
# Para este exemplo, vamos mantê-la aqui.
qdrant_client = QdrantService(
    host=config.QDRANT_HOST,
    port=config.QDRANT_PORT,
    api_key=config.QDRANT_API_KEY
)
 
embeddings_model = OpenAIEmbeddings(
    model="text-embedding-3-small",
    openai_api_key=config.OPENAI_API_KEY
)


def create_document_rag_tool(tenant_id: str, collection_name: str, documents: Document = None):
    """
    Factory Function que cria e retorna uma ferramenta de RAG configurada
    com um tenant_id e collection_name específicos.
    """

    def build_filter_list(documents: Document) -> List[Dict[str, Any]]:
        filters = []
        if documents.id_file:
            filters.append({"field": "metadata.id_file", "value": documents.id_file})
        return filters or None

    # Constrói o filtro a partir do Document
    filter_list = build_filter_list(documents)

 
    # A ferramenta real que o agente irá usar é definida DENTRO da factory.
    # Note que a assinatura dela tem apenas os parâmetros que a IA deve preencher.
    #@tool
    def document_rag_tool(query: str, top_k: int = 5):
        """
        Realiza busca semântica em uma coleção de documentos no Qdrant usando uma consulta em linguagem natural.
        Use esta ferramenta para encontrar informações em documentos armazenados, caso o usuário peça informações de documento.
        """
        print(f"--- Executando RAG para Tenant: {tenant_id}, Coleção: {collection_name} ---")
        try:
            query_vector = embeddings_model.embed_query(query)
            # Os parâmetros fixos (tenant_id, collection_name) são injetados aqui
            # a partir do escopo da função factory.
            search_kwargs = {
                "tenant_id": tenant_id,
                "collection_name": collection_name,
                "query_vector": query_vector,
                "top_k": top_k,
                "filter": filter_list
            }
 
            response = qdrant_client.search_vectors(**search_kwargs)
            print(f"--- RAG Resultados: {len(response)} documentos encontrados ---")
            return response
 
        except Exception as e:
            return {
                "error": f"Erro ao buscar documentos: {str(e)}",
                "results": []
            }
 
    # A factory retorna a função-ferramenta pronta e configurada
    return document_rag_tool