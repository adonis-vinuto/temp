from collections import defaultdict
from langchain_core.callbacks import BaseCallbackHandler
from langchain_core.outputs import LLMResult
from typing import Any, Dict

class UsageCallback(BaseCallbackHandler):
    """
    Callback aprimorado que agrega o uso de tokens POR MODELO e calcula o total geral.
    Usa chaves com UNDERSCORE para ser compatível com modelos Pydantic.
    """
    def __init__(self):
        super().__init__()
        # Usamos defaultdict para inicializar com chaves de UNDERSCORE
        self.usage_by_model = defaultdict(lambda: {
            "input_tokens": 0, 
            "output_tokens": 0, 
            "total_tokens": 0
        })

    def on_llm_end(self, response: LLMResult, **kwargs: Any) -> Any:
        if not (llm_output := response.llm_output):
            return

        model_name = llm_output.get("model_name", "unknown_model")
        
        if usage := llm_output.get("token_usage"):
            # Os dados do provedor LLM já vêm com underscore
            self.usage_by_model[model_name]["input_tokens"] += usage.get("prompt_tokens", 0)
            self.usage_by_model[model_name]["output_tokens"] += usage.get("completion_tokens", 0)
            self.usage_by_model[model_name]["total_tokens"] += usage.get("total_tokens", 0)

    def get_usage_dict(self) -> Dict[str, Any]:
        """Retorna os dados coletados com um detalhamento por modelo e um total geral."""
        
        # Garante que a soma também use chaves com UNDERSCORE
        grand_total = {
            "input_tokens": sum(data["input_tokens"] for data in self.usage_by_model.values()),
            "output_tokens": sum(data["output_tokens"] for data in self.usage_by_model.values()),
            "total_tokens": sum(data["total_tokens"] for data in self.usage_by_model.values()),
        }

        return {
            "usage_breakdown_by_model": dict(self.usage_by_model),
            "grand_total_usage": grand_total
        }