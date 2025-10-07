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

    def collection_exists(
            self, 
            tenant_id: str, 
            collection_name: str) -> bool:
        try:   
            return self.client.get_collection(collection_name) is not None
        
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
                
                self.client.recreate_collection(
                    collection_name=collection_name,
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
                self.client.delete_collection(collection_name)
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

            
            points = []

            for doc in documents:
                if hasattr(doc, 'dict'):
                    doc_dict = doc.dict()
                else:
                    doc_dict = doc

                point = PointStruct(
                    id=doc_dict["id"],
                    vector=doc_dict["vector"],
                    payload={
                        "text": doc_dict["text"],
                        "metadata": doc_dict["metadata"],
                        "tenant_id": tenant_id
                    }
                )

                points.append(point)
            
            self.client.upsert(
                collection_name=collection_name,
                points=points
            )
            
            logger.info(f"Inseridos {len(points)} documentos na collection '{collection_name}'")
            
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


        try:
            self.client.delete(
                collection_name=collection_name,
                points=point_ids
            )
            logger.info(f"Deleted points from collection '{collection_name}'.")

            if wait:
                self.client.wait_for_collection(collection_name)
                logger.info(f"All deletions processed in collection '{collection_name}'.")

        except Exception as e:
            logger.error(f"Failed to delete vectors from collection '{collection_name}': {e}")
            raise

    def delete_vectors_by_filter(
        self,
        tenant_id: str,
        collection_name: str,
        id_agent: str,
        id_file: str,
        wait: bool = True
    ) -> Dict[str, Any]:
        """
        Exclui a *coleção inteira* (não apenas por filtro) conforme a doc:
        https://api.qdrant.tech/api-reference/collections/delete-collection

        Observação: os parâmetros id_agent e id_file não são usados aqui,
        pois a operação remove a coleção completa.
        """
        if not self.collection_exists(tenant_id, collection_name):
            raise ValueError(f"Collection '{collection_name}' does not exist for tenant '{tenant_id}'.")


        try:
            # Conta pontos existentes (exato) para reportar no retorno
            count_resp = self.client.count(collection_name=collection_name, exact=True)

            # Extrai o número de pontos de forma resiliente a diferentes formatos
            deleted_count = 0
            if hasattr(count_resp, "count"):
                deleted_count = int(getattr(count_resp, "count", 0))
            elif isinstance(count_resp, dict):
                # formato REST-like: {"status":"ok","result":{"count":N}}
                deleted_count = int(count_resp.get("result", {}).get("count", 0))

            # Tempo de espera: a API aceita "timeout" em segundos
            timeout_sec = 60 if wait else None

            delete_resp = self.client.delete_collection(
                collection_name=collection_name,
                timeout=timeout_sec
            )

            # Normaliza o retorno: o client Python costuma devolver bool
            # True => sucesso; False/None => falha
            if isinstance(delete_resp, bool):
                status_value = "ok" if delete_resp else "error"
                operation_id = None
            elif isinstance(delete_resp, dict):
                status_value = str(delete_resp.get("status", "ok" if delete_resp.get("result") else "error"))
                operation_id = delete_resp.get("operation_id")
            else:
                # fallback para objetos com atributo .status/.operation_id (caso alguma versão traga isso)
                status_value = getattr(getattr(delete_resp, "status", None), "value", None) or \
                            str(getattr(delete_resp, "status", "ok"))
                operation_id = getattr(delete_resp, "operation_id", None)

            return {
                "status": status_value,
                "operation_id": operation_id,
                "deleted_count": deleted_count,
                "collection": collection_name,
            }

        except Exception as e:
            logger.error(
                "Failed to delete collection '%s': %s",
                collection_name,
                e
            )
            raise

    
    def search_vectors(
        self, 
        tenant_id: str, 
        collection_name: str, 
        query_vector: List[float], 
        top_k: int = 10, 
        filter: Optional[List[Dict[str, Any]]] = None
    ):
        
        try:
            search_params = {
                "collection_name": collection_name,
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