import os
from typing import List, Iterable
from openai import OpenAI
from ..configs import config

class EmbeddingService:
    def __init__(self, model_name: str = "text-embedding-3-small"):
        self.model = model_name
        self.openai_client = OpenAI(api_key=config.OPENAI_API_KEY)

    def _normalize_text(self, item) -> str:
        """
        Normalize an input item into a string for the embeddings API.
        Accepts str, dicts with a 'text' key, numbers, etc.
        Returns an empty string for None/empty inputs.
        """
        if item is None:
            return ""
        
        if isinstance(item, str):
            return item.strip()
        
        if isinstance(item, dict):
            # common case: {'text': '...'}
            val = item.get("text") or ""
            return str(val).strip()
        
        # fallback: convert to string
        return str(item).strip()

    def generate_embedding(self, text: str) -> List[float]:
        normalized = self._normalize_text(text)

        if not normalized:
            raise ValueError("Cannot generate embedding for empty text")

        try:
            response = self.openai_client.embeddings.create(
                input=normalized,
                model=self.model
            )

            return response.data[0].embedding

        except Exception as e:
            raise RuntimeError(f"Erro ao gerar embedding com OpenAI: {e}")

    def generate_embeddings(self, texts: Iterable) -> List[List[float]]:
        # Normalize and filter inputs
        normalized_texts: List[str] = []

        for item in texts:
            t = self._normalize_text(item)
            if t:
                normalized_texts.append(t)

        if not normalized_texts:
            raise ValueError("Nenhum texto válido para gerar embeddings (all items empty or invalid)")

        try:
            response = self.openai_client.embeddings.create(
                input=normalized_texts,
                model=self.model
            )

            return [item.embedding for item in response.data]

        except Exception as e:
            raise RuntimeError(f"Erro ao gerar embeddings com OpenAI: {e}")
