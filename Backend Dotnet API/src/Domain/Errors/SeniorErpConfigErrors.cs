using ErrorOr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Errors;

public static class SeniorErpConfigErrors
{
    public static readonly Error AlreadyRegistered = Error.Conflict(
        code: "SeniorErpConfig.AlreadyRegistered",
        description: "Já existe uma configuração cadastrada."
    );
    
    public static readonly Error NotFound = Error.NotFound(
        code: "SeniorErpConfig.NotFound",
        description: "Configuração não encontrada."
    );
    
    public static readonly Error SeniorErpConfigAlreadyExists = Error.Conflict(
        code: "SeniorErpConfig.AlreadyExists",
        description: "Já existe uma configuração do Senior Erp com o mesmo IdAgent e IdKnowledge."
    );

}
