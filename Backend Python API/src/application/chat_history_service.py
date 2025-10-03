import os
from typing import List, Optional
from langchain_core.messages import BaseMessage, HumanMessage, AIMessage, SystemMessage, ToolMessage
from fastapi import HTTPException
from langgraph.checkpoint.mysql.aio import AIOMySQLSaver

from ..domain.chat import MessageModel, MessageType
from ..infrastructure.checkpointer import create_async_mysql_checkpointer
class ChatHistoryService:
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

    async def handle(self, id_session: str) -> List[MessageModel]:
        """
        Busca e retorna o histórico de mensagens formatadas de uma thread.
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
            return []

        # Acessa o estado salvo no checkpoint
        checkpoint_data = checkpoint_tuple.checkpoint
        channel_values = checkpoint_data.get("channel_values", {})
        raw_messages = channel_values.get("messages", [])
        
        # Converte as mensagens do LangChain para o formato MessageModel
        formatted_messages = []
        for msg in raw_messages:
            message_type = self._get_message_type(msg)
            if message_type:  # Apenas adiciona se conseguir determinar o tipo
                formatted_message = MessageModel(
                    type=message_type,
                    content=msg.content
                )
                formatted_messages.append(formatted_message)
        
        return formatted_messages

    def _get_message_type(self, message: BaseMessage) -> Optional[MessageType]:
        """
        Determina o tipo da mensagem baseado na classe do LangChain.
        """
        # Verifica por instância (método principal)
        if isinstance(message, HumanMessage):
            return MessageType.HUMAN
        elif isinstance(message, AIMessage):
            return MessageType.AI
        elif isinstance(message, SystemMessage):
            return MessageType.SYSTEM
        elif isinstance(message, ToolMessage):
            return MessageType.TOOL
        
        # Fallback: verifica pelo nome da classe (para compatibilidade)
        class_name = message.__class__.__name__
        if class_name == "HumanMessage":
            return MessageType.HUMAN
        elif class_name == "AIMessage":
            return MessageType.AI
        elif class_name == "SystemMessage":
            return MessageType.SYSTEM
        elif class_name == "ToolMessage":
            return MessageType.TOOL
        
        # Retorna None para tipos desconhecidos
        return None

def get_chat_history_service():
    """Função factory para injeção de dependência do serviço."""
    return ChatHistoryService()