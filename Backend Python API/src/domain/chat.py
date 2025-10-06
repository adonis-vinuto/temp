from enum import Enum
from typing import List, Literal, Dict, Any, Optional
from langgraph.graph import MessagesState
from pydantic import BaseModel, ConfigDict, EmailStr, Field

class UserBase(BaseModel):
    name: str
    email: EmailStr

class UserInfo(UserBase):
    organization: str

class RoleEnum(int, Enum):
    """Define os papéis no chat: usuário ou agente."""

    USER = 0
    AGENT = 1

class ChatMessage(BaseModel):
    role: RoleEnum
    content: str

class Document(BaseModel):
    tenant_id: str 
    collection_name: str
    id_file: List[str]

class ChatRequest(BaseModel):
    id_session: str | None = None
    id_agent: str
    message: str
    preferences: Dict[str, str] | None = None
    module: Literal["People", "Sales", "Finance", "Support", "Tax"]
    user: UserBase
    organization: str
    # chat_history: List[ChatMessage] = Field(default_factory=list, alias="chat-history")
    documents: Optional[List[str]] = None 
    agent_type: Literal["basic"] = "basic"

    model_config = ConfigDict(populate_by_name=True)
    model_config['protected_namespaces'] = () # Evita a mensagem de warning para Pydantic v2

class UsageInfo(BaseModel):
    model_name: str = Field(alias="model-name")
    input_tokens: int = Field(alias="input-tokens")
    output_tokens: int = Field(alias="output-tokens")
    total_tokens: int = Field(alias="total-tokens")

    model_config = ConfigDict(populate_by_name=True)
    model_config['protected_namespaces'] = ()

class ChatResponse(BaseModel):
    id_session: str
    message_response: str = Field(alias="message-response")
    usage: Dict[str, Any]
    model_config = ConfigDict(populate_by_name=True)
    model_config['protected_namespaces'] = ()

class AgentState(MessagesState):
    prompt: str 
    user: UserInfo
    module: str
    preferences: Dict[str, str] | None
    document: Document

class MessageType(str, Enum):
    """Enum para os tipos de mensagem possíveis no histórico."""
    HUMAN = "human"
    AI = "ai"
    SYSTEM = "system"
    TOOL = "tool"

class MessageModel(BaseModel):
    """Response Model para uma única mensagem no histórico."""
    type: MessageType
    content: str
    
    model_config = ConfigDict(
        from_attributes=True,
        use_enum_values=True  # Para serializar o enum como string
    ) 