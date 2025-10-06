using ErrorOr;

namespace Domain.Errors;

public static class ChatHistoryErrors
{
    public static Error ChatHistoryNotFound => Error.NotFound(
        code: "ChatHistory.NotFound",
        description: "Histórico de chat não encontrado."
    );

    public static Error InvalidChatHistory => Error.Validation(
        code: "ChatHistory.Invalid",
        description: "Histórico de chat inválido."
    );
}