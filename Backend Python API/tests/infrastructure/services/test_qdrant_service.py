from unittest.mock import MagicMock

from src.infrastructure.qdrant.qdrant_service import QdrantService


def test_delete_vectors_by_file_uses_repository(monkeypatch):
    repository_mock = MagicMock()
    repository_mock.delete_vectors_by_filter.return_value = {
        "status": "completed",
        "deleted_count": 2,
        "operation_id": 77
    }

    service = QdrantService.__new__(QdrantService)
    service.client = repository_mock

    result = service.delete_vectors_by_file(
        tenant_id="Tenant",
        collection_name="Collection",
        id_agent="agent",
        id_file="file"
    )

    repository_mock.delete_vectors_by_filter.assert_called_once_with(
        "Tenant",
        "Collection",
        "agent",
        "file",
        True
    )
    assert result == {
        "status": "completed",
        "deleted_count": 2,
        "operation_id": 77
    }
