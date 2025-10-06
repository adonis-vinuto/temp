using ErrorOr;

namespace Domain.Errors;

public static class KnowledgeErrors
{
    public static readonly Error KnowledgeNotFound = Error.NotFound(
        code: "Knowledge.NotFound",
        description: "Knowledge não encontrado.");

    public static readonly Error KnowledgeWithoutPermission = Error.Conflict(
        code: "Knowledge.WithoutPermission",
        description: "Knowledge sem permissão.");
}