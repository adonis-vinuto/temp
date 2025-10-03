using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;

namespace Domain.Errors;
    public static class SeniorHcmConfigErrors
    {
        public static readonly Error AlreadyRegistered = Error.Conflict(
        code: "SeniorErpConfig.AlreadyRegistered",
        description: "Já existe uma configuração cadastrada."
    );
    public static readonly Error NotFound = Error.NotFound(
        code: "SeniorErpConfig.NotFound",
        description: "Configuração não encontrada."
    );
    }