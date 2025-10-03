"""Pydantic models for PDF extraction responses."""

from pydantic import BaseModel, ConfigDict, Field


class PageContent(BaseModel):
    """Represents the extracted text for a single PDF page."""

    text: str
    page: int


class PdfContentResponse(BaseModel):
    """Response model containing the extracted text of a PDF as pages."""

    pages: list[PageContent]

    model_config = ConfigDict(populate_by_name=True)

