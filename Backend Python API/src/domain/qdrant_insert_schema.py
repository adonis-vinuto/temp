from pydantic import BaseModel, Field
from typing import List
from uuid import uuid4

class QdrantMetadata(BaseModel):
    id_agent: str
    id_file: str
    type: str
    file_name: str
    page_number: int  

class QdrantInsertSchema(BaseModel):
    id: str = Field(default_factory=lambda: str(uuid4()))
    vector: List[float]
    text: str
    metadata: QdrantMetadata