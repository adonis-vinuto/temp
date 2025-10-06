using ErrorOr;

namespace Domain.Errors;

public static class FileErrors
{
    public static readonly Error UploadFail = Error.Failure(
        code: "File.UploadFail",
        description: "Erro ao enviar arquivo.");

    public static readonly Error NotFound = Error.NotFound(
        code: "File.NotFound",
        description: "Arquivo não encontrado.");

    public static readonly Error DeleteFail = Error.Failure(
        code: "File.DeleteFail",
        description: "Erro ao deletar arquivo.");
}