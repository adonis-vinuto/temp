from langgraph.graph import END, StateGraph
from langchain.chat_models import init_chat_model
from langchain_core.runnables import RunnableConfig

from src.domain.chat import AgentState
from src.domain.agent_prompts import get_hydrated_system_prompt
from src.application.agents.people_agent import people_node

llm = init_chat_model(model="openai/gpt-oss-120b", model_provider="groq", temperature=0)

def route_node(state: AgentState):
    doc_text = state.get("document", "")
    module = (state["module"], "")
 
    system_prompt = get_hydrated_system_prompt(
        module=module,
        user=state["user"],
        preferences=state.get("preferences")
    )

    return {"prompt": system_prompt}

def route_condition(state: AgentState):
    return state["module"].lower()

#TODO: Personalizar cada agente

def sales_node(state: AgentState, config: RunnableConfig):
    messages = [state["prompt"]] + state["messages"]
    response = llm.invoke(messages, config=config)
    return {"messages": [response]}

def finance_node(state: AgentState, config: RunnableConfig):
    messages = [state["prompt"]] + state["messages"]
    response = llm.invoke(messages, config=config)
    return {"messages": [response]}

def support_node(state: AgentState, config: RunnableConfig):
    messages = [state["prompt"]] + state["messages"]
    response = llm.invoke(messages, config=config)
    return {"messages": [response]}

def tax_node(state: AgentState, config: RunnableConfig):
    messages = [state["prompt"]] + state["messages"]
    response = llm.invoke(messages, config=config)
    return {"messages": [response]}

    
def graph_builder(checkpointer = None):
    builder = StateGraph(AgentState)
    builder.add_node("route", route_node)
    builder.add_node("people", people_node)
    builder.add_node("sales", people_node)
    builder.add_node("finance", people_node)
    builder.add_node("support", people_node)
    builder.add_node("tax", people_node)

    builder.set_entry_point("route")
    
    builder.add_conditional_edges(
        "route",
        route_condition,
        {
            "people": "people",
            "sales": "sales",
            "finance": "finance",
            "support": "support",
            "tax": "tax"
        }
    )

    builder.add_edge("people", END)
    builder.add_edge("sales", END)
    builder.add_edge("finance", END)
    builder.add_edge("support", END)
    builder.add_edge("tax", END)

    if checkpointer is not None:
        return builder.compile(checkpointer=checkpointer)
    else:
        return builder.compile()

langgraph_app = graph_builder()