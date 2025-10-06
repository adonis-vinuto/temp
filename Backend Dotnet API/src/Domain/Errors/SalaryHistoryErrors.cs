using ErrorOr;

namespace Domain.Errors;
public static class SalaryHistoryErrors
{
    public static readonly Error SalaryHistoryNotFound = Error.NotFound(
        code: "SalaryHistory.NotFound",
        description: "Histórico de salário não encontrado.");
}
