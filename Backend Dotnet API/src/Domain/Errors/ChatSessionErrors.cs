using ErrorOr;

namespace Domain.Errors;

public static class ChatSessionErrors
{
    public static Error ChatSessionNotFound => Error.NotFound(
        code: "ChatSession.NotFound",
        description: "Sessão de chat não encontrada."
    );

    public static Error ChatMessageNotFound => Error.NotFound(
        code: "ChatSession.MessageNotFound",
        description: "Mensagem de chat não encontrada."
    );

    public static Error SessionAgentMismatch => Error.Validation(
        code: "ChatSession.AgentMismatch",
        description: "A sessão não pertence ao agente especificado."
    );

    public static Error SessionExpired => Error.Validation(
        code: "ChatSession.Expired",
        description: "A sessão de chat expirou."
    );
}