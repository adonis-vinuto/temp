from typing import Optional

from pydantic import BaseModel, Field


class FileDeletionResponse(BaseModel):
    """Representa a resposta padronizada para remoção de vetores de um arquivo."""

    status: str = Field(..., description="Status da operação retornado pelo Qdrant")
    deleted_count: int = Field(0, ge=0, description="Quantidade de vetores removidos do Qdrant")
    operation_id: Optional[int] = Field(
        default=None,
        description="Identificador da operação retornado pelo Qdrant"
    )

