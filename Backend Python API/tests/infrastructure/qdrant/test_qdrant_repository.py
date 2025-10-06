from unittest.mock import MagicMock

import pytest
from qdrant_client.http import models

from src.infrastructure.qdrant.qdrant_repository import QdrantRepository


@pytest.fixture()
def repository(monkeypatch):
    repo = QdrantRepository.__new__(QdrantRepository)
    repo.host = "localhost"
    repo.port = 6333
    repo.api_key = None
    repo.client = MagicMock()

    monkeypatch.setattr(repo, "collection_exists", lambda tenant_id, collection_name: True)

    return repo


def test_delete_vectors_by_filter_builds_payload_filter(repository):
    repository.client.count.return_value = models.CountResult(count=5)
    repository.client.delete.return_value = models.UpdateResult(
        status=models.UpdateStatus.COMPLETED,
        operation_id=99
    )

    result = repository.delete_vectors_by_filter(
        tenant_id="TenantA",
        collection_name="CollectionB",
        id_agent="agent-123",
        id_file="file-456"
    )

    repository.client.count.assert_called_once()
    repository.client.delete.assert_called_once()

    delete_kwargs = repository.client.delete.call_args.kwargs

    assert delete_kwargs["collection_name"] == "tenanta_collectionb"
    assert delete_kwargs["wait"] is True

    payload_filter = delete_kwargs["filter"]
    assert isinstance(payload_filter, models.Filter)
    keys = {condition.key for condition in payload_filter.must}
    assert "metadata.id_agent" in keys
    assert "metadata.id_file" in keys

    assert result["status"] == models.UpdateStatus.COMPLETED.value
    assert result["operation_id"] == 99
    assert result["deleted_count"] == 5
