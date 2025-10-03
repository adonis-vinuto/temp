from langchain.chat_models import init_chat_model
from langchain_core.messages import BaseMessage, SystemMessage
from typing import List
from pydantic import BaseModel, Field

llm = init_chat_model(model="openai/gpt-oss-120b", model_provider="groq", temperature=0)

prompt = (
    "Você é um assistente especialista em criar títulos curtos. "
    "Resuma as duas primeiras mensagens da conversa em um título com poucas palavras e em portugues."
)

class ResponseModel(BaseModel):
    title: str = Field(description="O título resumido da conversa em texto plano.")

llm_with_structured_output = llm.with_structured_output(ResponseModel)

def summarize_chat(messages: List[BaseMessage]) -> str:
    # Pega apenas as duas primeiras mensagens
    first_two_messages = messages[:2]
    
    system_msg = SystemMessage(content=prompt)
    # Combina a mensagem do sistema com as duas primeiras mensagens
    messages_to_process = [system_msg] + first_two_messages
    
    # Use o modelo LLM para gerar um resumo a partir das duas primeiras mensagens
    response = llm_with_structured_output.invoke(messages_to_process)
    title = response.title

    return title
