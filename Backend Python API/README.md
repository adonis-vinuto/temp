# Projeto IA - Novo

API em FastAPI estruturada com DDD para um agente de chat baseado em LangGraph.

## Instalação

```bash
python -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
```

## Configuração

```bash
cp .env.example .env
```

Edite o arquivo `.env` e defina `GROQ_API_KEY` e outros segredos necessários.

## Execução

```bash
uvicorn src.main:app --reload
```

## Rota `/chat`

Recebe um JSON contendo mensagem, módulo (People, Sales, Finance, Support, Tax),
organização, dados do usuário, histórico de mensagens (`chat-history`) e uma
lista opcional de arquivos já convertidos em texto.

O agente utiliza uma estrutura de múltiplos passos com LangGraph para analisar
o contexto antes de gerar a resposta final, aumentando a precisão do retorno.

### Módulos disponíveis

- **People**: cuida de recursos humanos, recrutamento, benefícios,
  treinamento e relacionamento com colaboradores.
- **Sales**: foca em prospecção, gerenciamento de leads, negociação e
  relacionamento com clientes para gerar receita.
- **Finance**: responsável por orçamento, folha de pagamento, planejamento
  e relatórios financeiros.
- **Support**: oferece suporte ao cliente, solucionando problemas e
  mantendo uma base de conhecimento.
- **Tax**: garante conformidade com regulamentos fiscais, prepara
  declarações e orienta sobre estratégias tributárias.

Exemplo usando `curl`:

```bash
curl -X POST http://localhost:8000/chat/ \
  -H "Content-Type: application/json" \
  -d '{
    "user": {"name": "João", "email": "joao@example.com"},
    "organization": "MinhaEmpresa",
    "module": "Finance",
    "message": "Preciso de ajuda com minha folha de pagamento",
    "chat-history": [{"role":0,"content":"Mensagem anterior"}],
    "files": [{"name": "folha.txt", "content": "texto do arquivo"}]
  }'
```

Resposta esperada:

```json
{
  "message-response": "Texto gerado pelo agente...",
  "usage": {
    "model-name": "openai/gpt-oss-120b",
    "input-tokens": 2181,
    "output-tokens": 250,
    "total-tokens": 2431
  }
}
```

## Rota `/pdf`

Recebe um arquivo PDF e retorna apenas o conteúdo convertido em texto.

Exemplo usando `curl`:

```bash
curl -X POST http://localhost:8000/pdf/ \
  -F "file=@documento.pdf"
```

Resposta esperada:

```json
{
  "file-content": "Texto extraído do documento..."
}
```

## Rota `/file/{organization}/{id_agent}/{id_file}` (DELETE)

Remove todos os vetores associados a um arquivo específico do Qdrant.

```bash
curl -X DELETE http://localhost:8000/file/minha-org/agent-123/file-456
```

Resposta esperada:

```json
{
  "status": "completed",
  "deleted_count": 42,
  "operation_id": 12345
}
```

> O campo `deleted_count` reflete a quantidade estimada de vetores removidos
> do Qdrant com base no filtro aplicado.
