import sys
from pathlib import Path
from types import SimpleNamespace


ROOT_DIR = Path(__file__).resolve().parents[1]
if str(ROOT_DIR) not in sys.path:
    sys.path.insert(0, str(ROOT_DIR))


class _DummyChatGroq:
    def __init__(self, *args, **kwargs):
        pass

    def invoke(self, *args, **kwargs):
        return SimpleNamespace(content="")


def _dummy_camelot_read_pdf(*args, **kwargs):
    return SimpleNamespace(n=0)


if "langchain_groq" not in sys.modules:
    sys.modules["langchain_groq"] = SimpleNamespace(ChatGroq=_DummyChatGroq)

if "camelot" not in sys.modules:
    sys.modules["camelot"] = SimpleNamespace(read_pdf=_dummy_camelot_read_pdf)
