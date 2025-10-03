"""Service for extracting structured data from a document using an LLM."""

from __future__ import annotations

from langchain.schema import HumanMessage, SystemMessage
from langchain_groq import ChatGroq

from ..domain.chat import UsageInfo
from ..domain.document import DocumentSummaryRequest, DocumentSummaryResponse


class DocumentService:
    """Handle document summarization via sequential LLM calls."""

    def __init__(self, model: str = "openai/gpt-oss-120b", temperature: float = 0) -> None:
        self.llm = ChatGroq(model=model, temperature=temperature)

    def _ask(self, prompt: str, document: str) -> tuple[str, UsageInfo]:
        """Send a single prompt to the model and return the response and usage."""

        messages = [
            SystemMessage(
                content=(
                    "You are a helpful assistant. "
                    "Respond with exactly what is requested without extra text. "
                    "Responda sempre em português."
                )
            ),
            HumanMessage(content=f"Document:\n{document}\n{prompt}"),
        ]
        result = self.llm.invoke(messages)
        meta = result.response_metadata or {}
        token_usage = meta.get("token_usage", {})
        usage = UsageInfo(
            model_name=meta.get("model_name", ""),
            input_tokens=token_usage.get("input_tokens", 0),
            output_tokens=token_usage.get("output_tokens", 0),
            total_tokens=token_usage.get("total_tokens", 0),
        )
        return result.content.strip(), usage

    def handle(self, request: DocumentSummaryRequest) -> DocumentSummaryResponse:
        """Generate name, description and summary for the provided document."""

        name, usage1 = self._ask(
            "Retorne apenas o nome do documento em português.", request.file_content
        )
        description, usage2 = self._ask(
            "Forneça uma descrição breve do documento em no máximo um parágrafo, em português.",
            request.file_content,
        )
        resume, usage3 = self._ask(
            "Forneça um resumo do documento em no máximo seis parágrafos, em português.",
            request.file_content,
        )

        usage = UsageInfo(
            model_name=usage1.model_name,
            input_tokens=usage1.input_tokens + usage2.input_tokens + usage3.input_tokens,
            output_tokens=usage1.output_tokens + usage2.output_tokens + usage3.output_tokens,
            total_tokens=usage1.total_tokens + usage2.total_tokens + usage3.total_tokens,
        )

        return DocumentSummaryResponse(
            name_file=name,
            description_file=description,
            resume_file=resume,
            usage=usage,
        )
