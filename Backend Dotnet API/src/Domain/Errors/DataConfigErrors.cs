using ErrorOr;

namespace Domain.Errors;

public static class DataConfigErrors
{
    public static readonly Error DataConfigNotFound = Error.NotFound(
        code: "DataConfig.NotFound",
        description: "Data config não encontrado.");

    public static readonly Error DataConfigAlreadyRegistered = Error.Conflict(
        code: "DataConfig.AlreadyRegistered",
        description: "A Data Config já está registrada para esta organização.");

    public static readonly Error DatabaseConnectionFail = Error.Failure(
        code: "DataConfig.DatabaseConnectionFail",
        description: "Não foi possível conectar ao banco de dados.");
}