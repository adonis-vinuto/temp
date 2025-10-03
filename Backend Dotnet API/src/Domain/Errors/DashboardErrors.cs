using ErrorOr;

namespace Domain.Errors;

public static class DashboardErrors
{
    public static Error NotFound => Error.NotFound(
        code: "Dashboard.NotFound",
        description: "Dashboard não encontrado."
    );
}
