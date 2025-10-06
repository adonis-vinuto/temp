# Imagem base enxuta do Python
FROM python:3.11-slim

# Diretório de trabalho dentro do container
WORKDIR /app

# Instalar dependências do sistema necessárias para OCR e Pillow
RUN apt-get update && apt-get install -y \
    tesseract-ocr \
    tesseract-ocr-por \
    libgl1 \
    wget \
    && rm -rf /var/lib/apt/lists/*

# Copiar requirements e instalar libs Python
COPY requirements.txt .
RUN pip install --no-cache-dir --upgrade pip \
    && pip install --no-cache-dir -r requirements.txt \
    && pip install --no-cache-dir gunicorn

# Copiar o código do projeto
COPY . .

# Criar diretório de logs
RUN mkdir -p logs

# Variáveis de ambiente padrão
ENV PYTHONUNBUFFERED=1 \
    PYTHONPATH=/app \
    HOST=0.0.0.0 \
    PORT=8000 \
    WORKERS=4

# Expor a porta da API
EXPOSE 8000


# Comando padrão: Gunicorn + Uvicorn
CMD ["gunicorn", "src.main:app", "-k", "uvicorn.workers.UvicornWorker", "-b", "0.0.0.0:8000", "-w", "4"]
