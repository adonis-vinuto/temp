from langchain_core.callbacks import BaseCallbackHandler
from langchain_core.outputs import LLMResult
from typing import Any, Dict

class SimpleUsageCallback(BaseCallbackHandler):
    """
    Callback simplificado que agrega tokens e métricas de tempo em formato flat.
    """
    def __init__(self):
        super().__init__()
        self.completion_tokens = 0
        self.prompt_tokens = 0
        self.total_tokens = 0
        self.completion_time = 0.0
        self.prompt_time = 0.0
        self.queue_time = 0.0
        self.total_time = 0.0

    def on_llm_end(self, response: LLMResult, **kwargs: Any) -> Any:
        if not (llm_output := response.llm_output):
            return
        
        if usage := llm_output.get("token_usage"):
            self.prompt_tokens += usage.get("prompt_tokens", 0)
            self.completion_tokens += usage.get("completion_tokens", 0)
            self.total_tokens += usage.get("total_tokens", 0)
            
            # Captura métricas de tempo se disponíveis
            self.completion_time += usage.get("completion_time", 0.0)
            self.prompt_time += usage.get("prompt_time", 0.0)
            self.queue_time += usage.get("queue_time", 0.0)
            self.total_time += usage.get("total_time", 0.0)

    def get_usage_dict(self) -> Dict[str, Any]:
        """Retorna os dados em formato simples."""
        return {
            "completion_tokens": self.completion_tokens,
            "prompt_tokens": self.prompt_tokens,
            "total_tokens": self.total_tokens,
            "completion_time": self.completion_time,
            "prompt_time": self.prompt_time,
            "queue_time": self.queue_time,
            "total_time": self.total_time
        }
