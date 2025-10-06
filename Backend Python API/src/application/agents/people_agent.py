from langchain.chat_models import init_chat_model
from langgraph.prebuilt import create_react_agent
from langchain_core.runnables import RunnableConfig

from src.domain.chat import AgentState
from src.application.agents.tools.document_rag_tool import create_document_rag_tool

llm = init_chat_model(model="openai/gpt-oss-120b", model_provider="groq", temperature=0)

def people_node(state: AgentState, config: RunnableConfig):

    # Só adiciona a ferramenta se os parâmetros estiverem presentes
    if state["document"].tenant_id and state["document"].collection_name:
        specific_rag_tool = create_document_rag_tool(
            tenant_id=state["document"].tenant_id,
            collection_name=state["document"].collection_name,
            documents=state["document"]
        )
        tools = [specific_rag_tool]
    else:
        tools = []

    rag_agent = create_react_agent(
        model=llm,
        tools=tools
    )

    messages = [state["prompt"]] + state["messages"]

    result = rag_agent.invoke({"messages": messages}, config=config)
    agent_response = result["messages"][-1]

    return {"messages": [agent_response]}