from fastapi import FastAPI
from fastapi.testclient import TestClient

from src.api.file_router import router


def create_app():
    app = FastAPI()
    app.include_router(router)
    return app


def test_delete_file_vectors_success(monkeypatch):
    class DummyService:
        def __init__(self, *args, **kwargs):
            pass

        def delete_vectors_by_file(self, tenant_id, collection_name, id_agent, id_file):
            return {
                "status": "completed",
                "deleted_count": 3,
                "operation_id": 123
            }

    monkeypatch.setattr("src.api.file_router.QdrantService", DummyService)

    app = create_app()
    client = TestClient(app)

    response = client.delete("/file/org-1/agent-2/file-3")

    assert response.status_code == 200
    assert response.json() == {
        "status": "completed",
        "deleted_count": 3,
        "operation_id": 123
    }


def test_delete_file_vectors_connection_error(monkeypatch):
    class DummyService:
        def __init__(self, *args, **kwargs):
            pass

        def delete_vectors_by_file(self, *args, **kwargs):
            raise ConnectionError("boom")

    monkeypatch.setattr("src.api.file_router.QdrantService", DummyService)

    app = create_app()
    client = TestClient(app)

    response = client.delete("/file/org-1/agent-2/file-3")

    assert response.status_code == 503
    assert "boom" in response.json()["detail"]
