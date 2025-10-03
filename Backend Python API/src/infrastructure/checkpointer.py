from typing import Optional, Awaitable, Callable
from langgraph.checkpoint.mysql.aio import AIOMySQLSaver

async def create_async_mysql_checkpointer(db_uri: str) -> AIOMySQLSaver:
    """
    Retorna uma instância viva de AIOMySQLSaver (mantém a conexão aberta).
    Observação: este método entra no context manager e não o fecha.
    Feche em shutdown chamando: await checkpointer._close(None, None, None)
    """
    cm = AIOMySQLSaver.from_conn_string(db_uri)  # async context manager
    saver = await cm.__aenter__()                # entra e obtém a instância
    try:
        await saver.setup()
    except Exception:
        # se falhar no setup, fecha o context manager
        await cm.__aexit__(None, None, None)
        raise

    # guarda o método de fechamento para usar no shutdown da aplicação
    setattr(saver, "_close", cm.__aexit__)
    return saver