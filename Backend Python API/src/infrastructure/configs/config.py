import os
import dotenv
from pathlib import Path

dotenv.load_dotenv()

# Qdrant configuration com valores padrão
QDRANT_HOST = os.getenv("QDRANT_HOST", "aaaaaa")
QDRANT_PORT = int(os.getenv("QDRANT_PORT", "6333"))
QDRANT_API_KEY = os.getenv("QDRANT_API_KEY", None)

# OpenAI configuration
OPENAI_API_KEY = os.getenv("OPENAI_API_KEY", None)

# Groq configuration
GROQ_API_KEY = os.getenv("GROQ_API_KEY", None)

# Token for simple API authentication
TOKENGED = os.getenv("TOKENGED")
TOKENMIND = os.getenv("TOKENMIND")
