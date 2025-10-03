from fastapi import APIRouter

from ..application.document_service import DocumentService
from ..domain.document import DocumentSummaryRequest, DocumentSummaryResponse

router = APIRouter(prefix="/document", tags=["document"])
service = DocumentService()


@router.post("/", response_model=DocumentSummaryResponse)
async def document_summary_endpoint(request: DocumentSummaryRequest) -> DocumentSummaryResponse:
    """Receives raw document text and returns structured information."""
    return service.handle(request)
