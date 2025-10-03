"""Pydantic models for document summarization."""

from pydantic import BaseModel, ConfigDict, Field

from .chat import UsageInfo


class DocumentSummaryRequest(BaseModel):
    """Request model containing raw document text."""

    file_content: str = Field(alias="file-content")

    model_config = ConfigDict(populate_by_name=True)


class DocumentSummaryResponse(BaseModel):
    """Response model with structured information extracted from a document."""

    name_file: str = Field(alias="name-file")
    description_file: str = Field(alias="description-file")
    resume_file: str = Field(alias="resume-file")
    usage: UsageInfo

    model_config = ConfigDict(populate_by_name=True)
