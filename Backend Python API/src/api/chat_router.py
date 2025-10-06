from fastapi import APIRouter, Depends
from typing import List

from ..application.chat_service import ChatService, get_chat_service
from ..application.chat_history_service import ChatHistoryService, get_chat_history_service 
from ..application.chat_title_service import ChatTitleService, get_chat_title_service
from ..domain.chat import ChatRequest, ChatResponse, MessageModel

router = APIRouter(prefix="/chat", tags=["chat"])


@router.post("/", response_model=ChatResponse)
async def chat_endpoint(request: ChatRequest, service: ChatService = Depends(get_chat_service)) -> ChatResponse:
    """Processa a mensagem do usuário e retorna a resposta do agente."""
    return await service.handle(request)

@router.get("/{id_session}", response_model=List[MessageModel])
async def get_chat_history(id_session: str, service: ChatHistoryService = Depends(get_chat_history_service)) -> List[MessageModel]:
    """Retorna o histórico de mensagens de um thread específico."""
    return await service.handle(id_session)

@router.get("/title/{id_session}", response_model=str)
async def get_chat_title(id_session: str, service: ChatTitleService = Depends(get_chat_title_service)) -> str:
    """Retorna o título de um thread específico."""
    return await service.handle(id_session)