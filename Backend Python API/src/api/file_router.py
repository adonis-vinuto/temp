from fastapi import APIRouter, UploadFile, File, HTTPException, Depends
from typing import Dict, Any
import traceback
from ..infrastructure.qdrant.qdrant_service import QdrantService
from ..infrastructure.qdrant.naming import format_collection_name
from ..infrastructure.configs import config
from ..domain.file import FileDeletionResponse
from ..domain.file_summary import FileSummaryResponse
from ..infrastructure.qdrant.smart_document_extractor import extract_file_content_from_bytes
from ..infrastructure.qdrant.parse_qdrant_data import parse_qdrant_data

router = APIRouter(prefix="/file", tags=["file"])


@router.post("/summary", response_model=FileSummaryResponse)
async def summarize_file(file: UploadFile = File(...)) -> FileSummaryResponse:
    try:
        file_content = await file.read()
    except Exception as read_err:
        raise HTTPException(status_code=400, detail="Não foi possível ler o arquivo enviado.") from read_err

    if not file_content:
        raise HTTPException(status_code=400, detail="Arquivo enviado está vazio.")

    try:
        extracted_data, usage_data = extract_file_content_from_bytes(file_content, file.filename)
    except ValueError as value_err:
        raise HTTPException(status_code=400, detail=str(value_err)) from value_err
    except HTTPException:
        raise
    except Exception as extraction_err:
        raise HTTPException(status_code=500, detail="Erro ao processar o arquivo.") from extraction_err

    needs_fallback = False

    if file.filename.lower().endswith(".pdf"):
        first_page_text = ""
        if extracted_data.pages:
            first_page_text = extracted_data.pages[0].text.strip()
        if not first_page_text or len(first_page_text) <= 100:
            needs_fallback = True

    if needs_fallback:
        try:
            extracted_data, usage_data = extract_file_content_from_bytes(file_content, file.filename)
        except ValueError as value_err:
            raise HTTPException(status_code=400, detail=str(value_err)) from value_err
        except HTTPException:
            raise
        except Exception as fallback_err:
            raise HTTPException(status_code=500, detail="Erro ao processar o PDF.") from fallback_err

    if not extracted_data.resume.strip():
        raise HTTPException(status_code=422, detail="Não foi possível extrair um resumo do arquivo enviado.")

    return FileSummaryResponse(
        file_name=extracted_data.file_name,
        resume=extracted_data.resume,
    )


@router.get("/health/qdrant", response_model=Dict[str, Any])
async def check_qdrant_connection() -> Dict[str, Any]:
    """
    Endpoint para verificar a conectividade com o Qdrant
    """
    try:
        qdrant_client = QdrantService(
            host=config.QDRANT_HOST,
            port=config.QDRANT_PORT,
            api_key=config.QDRANT_API_KEY
        )
        
        collections = qdrant_client.client.get_collections()
        
        return {
            "status": "connected",
            "host": config.QDRANT_HOST,
            "port": config.QDRANT_PORT,
            "message": "Qdrant connection successful",
            "test_result": "OK"
        }
        
    except ConnectionError as conn_err:
        return {
            "status": "connection_failed",
            "host": config.QDRANT_HOST,
            "port": config.QDRANT_PORT,
            "error": str(conn_err),
            "message": "Failed to connect to Qdrant. Check if service is running."
        }
    except Exception as e:
        return {
            "status": "error",
            "host": config.QDRANT_HOST,
            "port": config.QDRANT_PORT,
            "error": str(e),
            "message": "Unexpected error connecting to Qdrant"
        }

@router.post("/{organization}/{id_agent}/{id_file}", response_model=Dict[str, Any])
async def extract_and_insert_file(
    organization: str,
    id_agent: str,
    id_file: str,
    file: UploadFile = File(...)) -> Dict[str, Any]:

    qdrant_client = QdrantService(
        host=config.QDRANT_HOST,
        port=config.QDRANT_PORT,
        api_key=config.QDRANT_API_KEY
    )

    file_content = await file.read()

    try:
        collection_name = format_collection_name(organization, id_agent, id_file)
    except ValueError as value_err:
        raise HTTPException(status_code=400, detail=str(value_err)) from value_err

    try:

        extracted_data, usage_data = extract_file_content_from_bytes(file_content, file.filename)

        parsed_data = parse_qdrant_data(extracted_data, id_agent, id_file)

        insertion_result = qdrant_client.insert_vectors(organization, collection_name, parsed_data)

        return {
            "message": "File processed and inserted successfully",
            "file_info": {
                "file_name": extracted_data.file_name,
                "resume": extracted_data.resume,
            },
            "qdrant_info": {
                "documents_inserted": len(parsed_data),
                "insertion_result": insertion_result
            },
            "usage": usage_data
        }
    
    except ConnectionError as conn_err:
        print(f"Qdrant connection error: {conn_err}")
        raise HTTPException(
            status_code=503, 
            detail=f"Qdrant service unavailable. Please check if Qdrant is running. Host: {config.QDRANT_HOST}:{config.QDRANT_PORT}"
        )
    except Exception as qdrant_err:
        print(f"Qdrant operation error: {qdrant_err}")
        print(f"Traceback: {traceback.format_exc()}")
        raise HTTPException(
            status_code=500, 
            detail=f"Error with Qdrant operations: {str(qdrant_err)}"
        )

@router.delete("/{organization}/{id_agent}/{id_file}", response_model=FileDeletionResponse)
async def delete_file_vectors(
    organization: str,
    id_agent: str,
    id_file: str
) -> FileDeletionResponse:

    qdrant_client = QdrantService(
        host=config.QDRANT_HOST,
        port=config.QDRANT_PORT,
        api_key=config.QDRANT_API_KEY
    )

    try:
        collection_name = format_collection_name(organization, id_agent, id_file)
    except ValueError as value_err:
        raise HTTPException(status_code=400, detail=str(value_err)) from value_err

    try:

        deletion_result = qdrant_client.delete_vectors_by_file(
            tenant_id=organization,
            collection_name=collection_name,
            id_agent=id_agent,
            id_file=id_file
        )

        return FileDeletionResponse(**deletion_result)

    except ConnectionError as conn_err:
        raise HTTPException(
            status_code=503,
            detail=f"Qdrant service unavailable. Host: {config.QDRANT_HOST}:{config.QDRANT_PORT}. Error: {conn_err}"
        ) from conn_err
    except ValueError as value_err:
        raise HTTPException(status_code=404, detail=str(value_err)) from value_err
    except Exception as qdrant_err:
        raise HTTPException(
            status_code=500,
            detail=f"Error deleting file vectors: {qdrant_err}"
        ) from qdrant_err
