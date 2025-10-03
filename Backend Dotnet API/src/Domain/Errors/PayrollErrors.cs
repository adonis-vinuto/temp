using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;


namespace Domain.Errors;

public static class PayrollErrors
{
    public static readonly Error PayrollNotFound = Error.NotFound(
        code: "Payroll.NotFound",
        description: "Payroll não encontrado.");

    public static Error EmployeeMissingRequiredKnowledge(string idKnowledge) => Error.Failure(
        code: "Payroll.EmployeeMissingRequiredKnowledge",
        description: $"O funcionário não possui o conhecimento obrigatório com Id '{idKnowledge}' para adicionar um registro de folha de pagamento.");
        

}
