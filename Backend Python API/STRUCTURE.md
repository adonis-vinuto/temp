# Estrutura do Projeto IA - Novo

Este documento descreve a organização do projeto e sugere melhorias para as próximas etapas.

## Arquivos principais

### `README.md`
Manual de instalação, execução e uso das rotas `/chat` e `/pdf`.
*Próximos passos:* incluir exemplos de respostas, seções de troubleshooting e instruções para produção.

### `requirements.txt`
Lista de dependências necessárias para executar a aplicação.
*Próximos passos:* fixar versões e adicionar ferramentas de qualidade de código como `ruff` e `mypy`.

### `src/main.py`
Ponto de entrada da aplicação FastAPI e registro das rotas.
*Próximos passos:* adicionar middleware de logging, tratamento global de erros e configurações externas.

### `src/api/chat_router.py`
Define a rota `/chat` que recebe dados em JSON e delega o processamento ao `ChatService`.
*Próximos passos:* validar campos obrigatórios e retornar códigos de erro mais descritivos.

### `src/api/pdf_router.py`
Rota `/pdf` responsável por receber arquivos PDF e devolver apenas o texto extraído.
*Próximos passos:* suportar outros formatos de documento e autenticação.

### `src/application/chat_service.py`
Orquestra o agente escolhido para responder às mensagens.
*Próximos passos:* permitir múltiplos tipos de agentes e aplicar caching ou streaming das respostas.

### `src/application/pdf_service.py`
Função utilitária que extrai texto de PDFs usando `pypdfium2` com fallback para OCR.
*Próximos passos:* tratar páginas muito extensas e adicionar suporte a outros idiomas.

### `src/application/agents/basic_agent.py`
Agente básico composto por duas etapas em LangGraph (análise e resposta) utilizando o modelo Groq.
*Próximos passos:* expandir para agentes especializados por módulo, adicionar memória de conversação e testes automatizados de prompts.

### `src/domain/chat.py`
Modelos Pydantic que representam documentos, informações do usuário e dados de requisição/resposta do chat.
*Próximos passos:* criar validações específicas por módulo e adicionar campos opcionais como idioma ou nível de prioridade.

### `src/domain/pdf.py`
Modelo de resposta que contém o conteúdo extraído de PDFs.
*Próximos passos:* incluir metadados adicionais como número de páginas e autores.

### `src/__init__.py`, `src/api/__init__.py`, `src/application/__init__.py`, `src/application/agents/__init__.py`, `src/domain/__init__.py`
Arquivos vazios usados para declarar os diretórios como pacotes Python.
*Próximos passos:* podem ser removidos caso se utilize Python 3.11+ com `namespace packages` ou receber inicializações específicas.

## Próximas melhorias gerais

- Adicionar testes unitários e de integração cobrindo os fluxos principais.
- Configurar observabilidade com LangSmith ou ferramentas similares.
- Criar camada de configuração por ambiente e suporte a variáveis de ambiente.
- Incluir CI para lint, testes e build automatizado.

