using ErrorOr;

namespace Domain.Errors;

public static class TwilioConfigErrors
{
    public static Error TwilioConfigNotFound => Error.NotFound(
        code: "TwilioConfig.NotFound",
        description: "Configuração do Twilio não encontrada.");

    public static Error TwilioConfigAlreadyExists => Error.Conflict(
        code: "TwilioConfig.AlreadyExists",
        description: "Já existe uma configuração do Twilio para este agente.");

    public static Error InvalidAccountSid => Error.Validation(
        code: "TwilioConfig.InvalidAccountSid",
        description: "Account SID do Twilio inválido.");

    public static Error InvalidAuthToken => Error.Validation(
        code: "TwilioConfig.InvalidAuthToken",
        description: "Auth Token do Twilio inválido.");

    public static Error InvalidWebhookUrl => Error.Validation(
        code: "TwilioConfig.InvalidWebhookUrl",
        description: "URL do Webhook inválida.");
}