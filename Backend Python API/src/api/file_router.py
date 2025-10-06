from fastapi import APIRouter, UploadFile, File, HTTPException, Depends
from typing import Dict, Any
import traceback
from ..infrastructure.qdrant.document_processor import DocumentProcessor
from ..infrastructure.qdrant.qdrant_service import QdrantService
from ..infrastructure.configs import config
from ..application.pdf_service import extract_text_from_pdf_bytes
from ..infrastructure.qdrant.chunck_qdrant import chunk_and_prepare_qdrant
from ..infrastructure.qdrant.extract_camelot import extract_camelot
from ..infrastructure.qdrant.text_refiner import text_file_name, text_resume

router = APIRouter(prefix="/file", tags=["file"])


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

    document_processor = DocumentProcessor(max_chunk_size=1000)

    file_content = await file.read()

    extracted_data = await document_processor.extract_text(file_content, file.filename)

    try:
        if (len(extracted_data.pages[0].text) <= 100):
            try:

                extracted_data = extract_text_from_pdf_bytes(file_content, file.filename)

                qdrant_documents = document_processor.process_file_for_qdrant(extracted_data, id_agent, id_file)
                    
                insertion_result = qdrant_client.insert_vectors(organization, id_agent, qdrant_documents)
                            
                return {
                    "message": "File processed and inserted successfully",
                    "file_info": {
                        "file_name": extracted_data.file_name,
                        "resume": extracted_data.resume,
                    },
                    "qdrant_info": {
                        "documents_inserted": len(qdrant_documents),
                        "insertion_result": insertion_result
                    }
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

        else:            
            try:

                extracted_pages = extract_camelot(file_content)

                resume = text_resume(extracted_pages)
                
                file_name = text_file_name(extracted_pages)

                qdrant_documents = chunk_and_prepare_qdrant(
                    pages=extracted_pages,
                    id_agent=id_agent,
                    id_file=id_file,
                    file_type="pdf",
                    file_name=file.filename
                )

                insertion_result = qdrant_client.insert_vectors(organization, id_agent, qdrant_documents)

                return {
                    "message": "File processed and inserted successfully",
                    "file_info": {
                        "file_name": file_name,
                        "resume": resume,
                    },
                    "qdrant_info": {
                        "documents_inserted": len(qdrant_documents),
                        "insertion_result": insertion_result
                    }
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
    
    except HTTPException:
        raise

    except Exception as e:
        print(f"General error processing file: {e}")
        print(f"Traceback: {traceback.format_exc()}")
        raise HTTPException(status_code=500, detail=f"Error processing file: {str(e)}")
