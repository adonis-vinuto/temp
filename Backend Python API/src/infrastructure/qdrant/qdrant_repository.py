from qdrant_client import QdrantClient
from qdrant_client.http.models import Distance, VectorParams, PointStruct
from qdrant_client.http.exceptions import UnexpectedResponse
from qdrant_client.http import models  # Importa os modelos do Qdrant
from typing import List, Dict, Any, Optional, Union
import logging

logger = logging.getLogger(__name__)

class QdrantRepository:
    def __init__(self, host: str, port: int, api_key: str):
        self.host = host
        self.port = port
        self.api_key = api_key

        self.client = None
        self.connect()

    def connect(self):
        try:
            self.client = QdrantClient(
                host=self.host,
                port=self.port,
                api_key=self.api_key,
                https=False,
                timeout=30
            )
            logger.info("Connected to Qdrant successfully.")
        except Exception as e:
            logger.error(f"Failed to connect to Qdrant: {e}")
            raise
    
    def collection_name(
            self, 
            tenant_id: str, 
            base_name: str) -> str:
        tenant_id = tenant_id.lower()
        base_name = base_name.lower()
        return f"{tenant_id}_{base_name}"

    def collection_exists(
            self, 
            tenant_id: str, 
            collection_name: str) -> bool:
        try:
            full_name = self.collection_name(tenant_id, collection_name)
            
            return self.client.get_collection(full_name) is not None
        
        except UnexpectedResponse as e:
            if e.status_code == 404:
                return False
            logger.error(f"Error checking collection existence: {e}")
            raise

    def create_collection(
            self, 
            tenant_id: str, 
            collection_name: str, 
            dimension: int, 
            distance: Distance = Distance.COSINE):
        if not self.collection_exists(tenant_id, collection_name):

            try:
                full_name = self.collection_name(tenant_id, collection_name)

                self.client.recreate_collection(
                    collection_name=full_name,
                    vectors_config=VectorParams(size=dimension, distance=distance)
                )

                logger.info(f"Collection '{collection_name}' created successfully.")
                return
            
            except Exception as e:
                logger.error(f"Failed to create collection '{collection_name}': {e}")
                raise

        else:
            logger.info(f"Collection '{collection_name}' already exists.")

    def delete_collection(
            self, 
            tenant_id: str, 
            collection_name: str):
        if self.collection_exists(tenant_id, collection_name):
        
            try:
                self.client.delete_collection(self.collection_name(tenant_id, collection_name))
                logger.info(f"Collection '{collection_name}' deleted successfully.")

            except Exception as e:
                logger.error(f"Failed to delete collection '{collection_name}': {e}")
                raise

        else:
            logger.info(f"Collection '{collection_name}' does not exist.")

    def get_collections(self) -> List[str]:
        try:
            collections = self.client.get_collections().collections
            return [col.name for col in collections]
        
        except Exception as e:
            logger.error(f"Failed to retrieve collections: {e}")
            raise
    
    def list_tenants_collections(self, tenant_id: str) -> List[str]:
        try:
            all_collections = self.get_collections()
            tenant_collections = [col for col in all_collections if col.startswith(f"tenant_{tenant_id}_")]

            logger.info(f"Collections for tenant '{tenant_id}': {tenant_collections}")

            return tenant_collections
        
        except Exception as e:
            logger.error(f"Failed to list collections for tenant '{tenant_id}': {e}")
            raise

    def insert_vectors(
            self, 
            tenant_id: str, 
            collection_name: str, 
            documents: List[Union[Dict[str, Any], Any]]):
        """Insere vetores na collection"""
        try:
            if not self.collection_exists(tenant_id, collection_name):
                self.create_collection(tenant_id, collection_name, dimension=1536)

            full_name = self.collection_name(tenant_id, collection_name)
            
            points = []

            for doc in documents:
                if hasattr(doc, 'dict'):
                    doc_dict = doc.dict()

                    point = PointStruct(
                        id=doc_dict["id"],
                        vector=doc_dict["vector"],
                        payload={
                            "text": doc_dict["text"],
                            "metadata": doc_dict["metadata"],
                            "tenant_id": tenant_id
                        }
                    )

                else:
                    point = PointStruct(
                        id=doc["id"],
                        vector=doc["vector"],
                        payload=doc.get("payload", {})
                    )

                points.append(point)
            
            self.client.upsert(
                collection_name=full_name,
                points=points
            )
            
            logger.info(f"Inseridos {len(points)} documentos na collection '{full_name}'")
            
        except Exception as e:
            logger.error(f"Erro ao inserir vetores: {e}")
            raise

    def delete_vectors(
            self, 
            tenant_id: str, 
            collection_name: str, 
            point_ids: List[int], 
            wait: bool = True):
        if not self.collection_exists(tenant_id, collection_name):
            raise ValueError(f"Collection '{collection_name}' does not exist for tenant '{tenant_id}'.")

        full_name = self.collection_name(tenant_id, collection_name)

        try:
            self.client.delete(
                collection_name=full_name,
                points=point_ids
            )
            logger.info(f"Deleted points from collection '{collection_name}'.")

            if wait:
                self.client.wait_for_collection(full_name)
                logger.info(f"All deletions processed in collection '{collection_name}'.")

        except Exception as e:
            logger.error(f"Failed to delete vectors from collection '{collection_name}': {e}")
            raise
    
    def search_vectors(
        self, 
        tenant_id: str, 
        collection_name: str, 
        query_vector: List[float], 
        top_k: int = 10, 
        filter: Optional[List[Dict[str, Any]]] = None
    ):
        full_name = self.collection_name(tenant_id, collection_name)
        
        try:
            search_params = {
                "collection_name": full_name,
                "query_vector": query_vector,
                "limit": top_k
            }
            if filter:
                must_conditions = []
                for f in filter:
                    if "field" in f and "value" in f:
                        # se value for lista → MatchAny
                        if isinstance(f["value"], list):
                            must_conditions.append(
                                models.FieldCondition(
                                    key=f["field"],
                                    match=models.MatchAny(any=f["value"])
                                )
                            )
                        else:
                            must_conditions.append(
                                models.FieldCondition(
                                    key=f["field"],
                                    match=models.MatchValue(value=f["value"])
                                )
                            )

                query_filter = models.Filter(must=must_conditions)
                search_params["query_filter"] = query_filter  # <-- agora suporta os dois casos!

            results = self.client.search(**search_params)
            logger.info(f"Search completed in collection '{collection_name}' with top_k={top_k}.")
            return [result.dict() for result in results]

        except Exception as e:
            logger.error(f"Failed to search vectors in collection '{collection_name}': {e}")
            raise