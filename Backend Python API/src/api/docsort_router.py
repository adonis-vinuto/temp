from fastapi import APIRouter, UploadFile, File, Form, HTTPException
import json
from typing import List, Dict, Any

from ..application.docsort_service import DocsortService
from ..domain.docsort import DocsortRequest, DocsortResponse
from ..infrastructure.advanced_ocr_processor import create_advanced_pdf_extractor

router = APIRouter(prefix="/docsort", tags=["docsort"])
ocr_processor = create_advanced_pdf_extractor()
service = DocsortService(ocr_processor=ocr_processor)


@router.post("/", response_model=DocsortResponse)
async def docsort_endpoint(
    file: UploadFile = File(..., description="Arquivo PDF para processamento"),
    fields: str = Form(..., description="Lista de campos em formato JSON")
) -> DocsortResponse:
    """Recebe um arquivo PDF e campos em JSON, retorna informações estruturadas extraídas via IA"""
    
    # Validação do arquivo
    if not file.filename or not file.filename.lower().endswith('.pdf'):
        raise HTTPException(status_code=400, detail="Apenas arquivos PDF são aceitos.")
    
    # Parse do JSON dos campos
    try:
        fields_list = json.loads(fields)
        if not isinstance(fields_list, list):
            raise HTTPException(status_code=400, detail="Fields deve ser uma lista JSON.")
    except json.JSONDecodeError:
        raise HTTPException(status_code=400, detail="Fields deve ser um JSON válido.")
    
    # Cria o objeto de request
    request = DocsortRequest(file=file, fields=fields_list)
    
    try:
        return await service.handle(request)
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Erro no processamento: {str(e)}")
