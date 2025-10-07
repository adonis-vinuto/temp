import os
import asyncio
from uuid import uuid4
from langchain.schema import AIMessage, HumanMessage
 
from ..domain.chat import ChatRequest, ChatResponse, UserInfo, AgentState, Document
from ..infrastructure.qdrant.naming import format_collection_name
from ..infrastructure.usage_callback import UsageCallback
from .agents.agent_graph import graph_builder
from ..infrastructure.checkpointer import create_async_mysql_checkpointer
 
class ChatService:
    _init_lock = asyncio.Lock()
    _app = None  # grafo compilado (async checkpointer)
    _thread_safe_ready = False
 
    async def _ensure_app(self):
        if self._thread_safe_ready:
            return
        async with self._init_lock:
            if self._thread_safe_ready:
                return
            mysql_uri = os.getenv("MYSQL_URI")  # ex: mysql+aiomysql://app:app@localhost:3306/langgraph
            checkpointer = None
            if mysql_uri:
                checkpointer = await create_async_mysql_checkpointer(mysql_uri)
            # Compila com o checkpointer (async-safe)
            type(self)._app = graph_builder(checkpointer=checkpointer)
            type(self)._thread_safe_ready = True
 
    async def handle(self, request: ChatRequest) -> ChatResponse:
        await self._ensure_app()
 
        usage_cb = UsageCallback()
        id_session = getattr(request, "id_session", None) or str(uuid4())
 
        config = {
            "callbacks": [usage_cb],
            "configurable": {"thread_id": id_session},
        }
 
        user = UserInfo(
            name=request.user.name,
            email=request.user.email,
            organization=request.organization
        )

        if request.documents is not None:
            collection_name = ""
            if request.documents:
                try:
                    collection_name = format_collection_name(request.organization, request.id_agent, request.documents[0])
                except ValueError:
                    collection_name = ""

            doc = Document(
                tenant_id=request.organization,
                collection_name=collection_name,
                id_file=request.documents
            )
        else:
            doc = Document(
                tenant_id=" ",
                collection_name=" ",
                id_file=[]
            )

        print("Document in ChatService:", doc)

        input_msgs = [HumanMessage(content=request.message)]
 
        initial_state = AgentState(
            messages=input_msgs,
            user=user,
            module=(request.module or ""),   # route_condition normaliza
            document=doc,
            preferences=(request.preferences or None)
        )
 
        final_state = await type(self)._app.ainvoke(initial_state, config)
        last_ai = next((m for m in reversed(final_state["messages"]) if isinstance(m, AIMessage)), None)
        response_text = last_ai.content if last_ai else ""
        usage = usage_cb.get_usage_dict()

        return ChatResponse(id_session=id_session, message_response=response_text, usage=usage)

def get_chat_service():
    return ChatService()