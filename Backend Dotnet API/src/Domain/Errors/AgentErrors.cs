using ErrorOr;

namespace Domain.Errors;

public static class AgentErrors
{
    public static Error AgentNotFound => Error.NotFound(
        code: "Agent.NotFound",
        description: "Agente não encontrado."
    );

    public static Error AgentAlreadyExists => Error.Conflict(
        code: "Agent.AlreadyExists",
        description: "Agente já existe com este identificador."
    );

    public static Error AgentInactive => Error.Validation(
        code: "Agent.Inactive",
        description: "O agente está inativo."
    );

    public static Error InvalidAgentModule => Error.Validation(
        code: "Agent.InvalidModule",
        description: "O agente não pertence ao módulo especificado."
    );
}