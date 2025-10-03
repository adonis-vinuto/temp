using ErrorOr;

namespace Domain.Errors;

public static class EmployeeErrors
{
    public static readonly Error EmployeeNotFound = Error.NotFound(
        code: "Employee.NotFound",
        description: "Funcionário não encontrado.");
}