"""Utility helpers for constructing Qdrant collection identifiers."""


def format_collection_name(id_agent: str, id_file: str) -> str:
    """Builds a normalized collection name using the agent and file identifiers."""

    def _normalize(value: str) -> str:
        return value.strip().replace(" ", "_").lower()

    normalized_parts = [
        _normalize(part)
        for part in (id_agent, id_file)
        if part and part.strip()
    ]

    if len(normalized_parts) != 2:
        raise ValueError("Both id_agent and id_file must be provided to format the collection name.")

    return "_".join(normalized_parts)
