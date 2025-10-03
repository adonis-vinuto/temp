"""Basic multi-agent implementation using LangGraph.

This agent performs two steps:
1. An analysis agent drafts a plan using the provided context.
2. A responder agent generates the final answer based on the analysis.

Both agents use the Groq model via LangChain and are orchestrated with
LangGraph for clearer reasoning.
"""

import sys
from pathlib import Path
from typing import Dict, List, TypedDict

from langchain.schema import AIMessage, HumanMessage, SystemMessage
from langchain_groq import ChatGroq
from langgraph.graph import END, StateGraph

# Adiciona o diretório raiz do projeto ao sys.path para permitir importações
project_root = Path(__file__).parent.parent.parent.parent
sys.path.insert(0, str(project_root))

from src.domain.chat import ChatMessage, Document, RoleEnum, UsageInfo, UserInfo


MODULE_KNOWLEDGE: Dict[str, str] = {
    "People": (
        "Department responsible for human resources, handling recruitment, "
        "benefits, training and employee relations."
    ),
    "Sales": (
        "Focuses on prospecting, lead management, negotiating deals and "
        "maintaining customer relationships to drive revenue."
    ),
    "Finance": (
        "Manages budgets, payroll, financial planning and reporting to "
        "ensure the company's fiscal health."
    ),
    "Support": (
        "Provides assistance to customers by troubleshooting issues, "
        "managing tickets and maintaining a knowledge base."
    ),
    "Tax": (
        "Ensures compliance with tax regulations, prepares filings and "
        "advises on tax-efficient strategies."
    ),
}


class AgentState(TypedDict, total=False):
    message: str
    module: str
    document: Document | None
    user: UserInfo
    chat_history: List[ChatMessage]
    analysis: str
    usage: UsageInfo
    response: str


class BasicAgent:
    """Agente básico composto por múltiplos passos utilizando LangGraph."""

    def __init__(self, model: str = "openai/gpt-oss-120b", temperature: float = 0) -> None:
        self.llm = ChatGroq(model=model, temperature=temperature)

        workflow: StateGraph[AgentState] = StateGraph(AgentState)

        workflow.add_node("analyze", self._analyze)
        workflow.add_node("respond", self._respond)
        workflow.set_entry_point("analyze")
        workflow.add_edge("analyze", "respond")
        workflow.add_edge("respond", END)

        self.graph = workflow.compile()

    def _analyze(self, state: AgentState) -> Dict[str, str]:
        doc_text = state["document"].content if state.get("document") else ""
        module_info = MODULE_KNOWLEDGE.get(state["module"], "")
        system = (
            f"You are an expert planner for the {state['module']} department. "
            f"{module_info} User: {state['user'].name} from {state['user'].organization}. "
            "Responda sempre em português."
        )
        if doc_text:
            system += f"\nUse the following document as context:\n{doc_text}"
        messages = [SystemMessage(content=system)]
        for msg in state.get("chat_history", []):
            if msg.role == RoleEnum.USER:
                messages.append(HumanMessage(content=msg.content))
            else:
                messages.append(AIMessage(content=msg.content))
        messages.append(HumanMessage(content=state["message"]))
        analysis = self.llm.invoke(messages).content
        return {"analysis": analysis}

    def _respond(self, state: AgentState) -> Dict[str, str]:
        module_info = MODULE_KNOWLEDGE.get(state["module"], "")
        system = (
            f"You are a helpful assistant for the {state['module']} department. "
            f"{module_info} User: {state['user'].name} from {state['user'].organization}. "
            "Provide a detailed and accurate answer in Portuguese."
        )
        messages = [SystemMessage(content=system + f"\nPrior analysis: {state['analysis']}")]
        for msg in state.get("chat_history", []):
            if msg.role == RoleEnum.USER:
                messages.append(HumanMessage(content=msg.content))
            else:
                messages.append(AIMessage(content=msg.content))
        messages.append(HumanMessage(content=state["message"]))
        result = self.llm.invoke(messages)
        meta = result.response_metadata or {}
        token_usage = meta.get("token_usage", {})
        usage = UsageInfo(
            model_name=meta.get("model_name", ""),
            input_tokens=token_usage.get("input_tokens", 0),
            output_tokens=token_usage.get("output_tokens", 0),
            total_tokens=token_usage.get("total_tokens", 0),
        )
        return {"response": result.content, "usage": usage}

    def run(
        self,
        message: str,
        module: str,
        user: UserInfo,
        chat_history: List[ChatMessage],
        document: Document | None,
    ) -> tuple[str, UsageInfo]:
        initial_state: AgentState = {
            "message": message,
            "module": module,
            "user": user,
            "chat_history": chat_history,
        }
        if document:
            initial_state["document"] = document
        result = self.graph.invoke(initial_state)
        return result["response"], result["usage"]

