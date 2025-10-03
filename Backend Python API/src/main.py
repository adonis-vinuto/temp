from fastapi import FastAPI
from dotenv import load_dotenv

load_dotenv()

from .api.chat_router import router as chat_router
from .api.document_router import router as document_router
from .api.docsort_router import router as docsort_router
from .api.file_router import router as file_router
from .infrastructure.auth.verify_token import verify_token

app = FastAPI(title="AI Agent API - Novo")

# Middleware to verify token for all routes except /health
app.middleware("http")(verify_token)

app.include_router(chat_router)
app.include_router(document_router)
app.include_router(docsort_router)
app.include_router(file_router)

@app.get("/health")
async def health() -> dict[str, str]:
    return {"status": "ok"}


# para rodar local: uvicorn src.main:app --reload