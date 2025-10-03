using ErrorOr;

namespace Domain.Errors;

public static class UserErrors
{
    public static readonly Error UserNotFound = Error.NotFound(
        code: "User.NotFound",
        description: "Usuário não encontrado.");
    
    public static readonly Error ModuleAccessDenied = Error.Forbidden(
        code: "User.ModuleAccessDenied",
        description: "Usuário não tem acesso a este módulo.");
}