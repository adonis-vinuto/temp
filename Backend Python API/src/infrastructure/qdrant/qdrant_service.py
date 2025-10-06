from typing import List, Dict, Any, Optional, Union
from src.infrastructure.qdrant.qdrant_repository import QdrantRepository, PointStruct, Distance
from ..configs.config import QDRANT_HOST, QDRANT_PORT, QDRANT_API_KEY

class QdrantService:
    def __init__(
        self,
        host: str = QDRANT_HOST,
        port: int = QDRANT_PORT,
        api_key: str = QDRANT_API_KEY
    ):
        self.client = QdrantRepository(host=host, port=port, api_key=api_key)

    def create_collection(self, tenant_id: str, collection_name: str, dimension: int, distance: Distance = Distance.COSINE):
        return self.client.create_collection(tenant_id, collection_name, dimension, distance)

    def insert_vectors(self, tenant_id: str, collection_name: str, documents: List[Union[Dict[str, Any], Any]]):
        return self.client.insert_vectors(tenant_id, collection_name, documents)

    def search_vectors(self, tenant_id: str, collection_name: str, query_vector: List[float], top_k: int = 10, filter: Optional[Dict[str, Any]] = None):
        return self.client.search_vectors(tenant_id, collection_name, query_vector, top_k, filter)

    def delete_vectors(self, tenant_id: str, collection_name: str, point_ids: List[int], wait: bool = True):
        return self.client.delete_vectors(tenant_id, collection_name, point_ids, wait)

    def delete_vectors_by_file(
        self,
        tenant_id: str,
        collection_name: str,
        id_agent: str,
        id_file: str,
        wait: bool = True
    ):
        return self.client.delete_vectors_by_filter(
            tenant_id,
            collection_name,
            id_agent,
            id_file,
            wait
        )

    def delete_collection(self, tenant_id: str, collection_name: str):
        return self.client.delete_collection(tenant_id, collection_name)

    def list_collections(self, tenant_id: str):
        return self.client.list_tenants_collections(tenant_id)
    
    def collection_exists(self, tenant_id: str, collection_name: str) -> bool:
        return self.client.collection_exists(tenant_id, collection_name)