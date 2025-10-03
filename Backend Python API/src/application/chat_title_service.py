import os
from typing import List, Optional
from langchain_core.messages import BaseMessage
from fastapi import HTTPException
from langgraph.checkpoint.mysql.aio import AIOMySQLSaver

from ..domain.chat import MessageModel, MessageType
from ..infrastructure.checkpointer import create_async_mysql_checkpointer
from .agents.chat_title_llm import summarize_chat

class ChatTitleService():
    def __init__(self):
        # A instância do checkpointer assíncrono
        self._checkpointer: Optional[AIOMySQLSaver] = None # Adicionei o tipo para clareza

    async def _ensure_checkpointer(self):
        """Garante que a instância do checkpointer seja criada apenas uma vez."""
        if self._checkpointer:
            return
        mysql_uri = os.getenv("MYSQL_URI")
        if not mysql_uri:
            raise RuntimeError("MYSQL_URI não configurado")
        # CORREÇÃO: await na criação do checkpointer
        self._checkpointer = await create_async_mysql_checkpointer(mysql_uri)

    async def handle(self, id_session: str) -> str:
        """
        Busca e retorna o histórico de mensagens no formato MessageState padrão do LangGraph.
        """
        await self._ensure_checkpointer()

        if not id_session:
            raise HTTPException(status_code=400, detail="id_session é obrigatório")

        config = {
            "configurable": {"thread_id": id_session},
        }

        # Busca o checkpoint tuple
        checkpoint_tuple = await self._checkpointer.aget_tuple(config=config)
        
        if not checkpoint_tuple:
            return ""

        # Acessa o estado salvo no checkpoint
        checkpoint_data = checkpoint_tuple.checkpoint
        channel_values = checkpoint_data.get("channel_values", {})
        
        # Retorna as mensagens no formato MessageState padrão (List[BaseMessage])
        messages = channel_values.get("messages", [])

        return summarize_chat(messages)

        

def get_chat_title_service():
    """Função factory para injeção de dependência do serviço."""
    return ChatTitleService()