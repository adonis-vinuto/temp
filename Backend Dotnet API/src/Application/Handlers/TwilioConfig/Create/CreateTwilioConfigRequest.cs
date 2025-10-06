using FluentValidation;

namespace Application.Handlers.TwilioConfig.Create;

public record CreateTwilioConfigRequest(
    Guid IdAgent,
    string AccountSid,
    string AuthToken,
    string WebhookUrl
);

public class CreateTwilioConfigRequestValidator : AbstractValidator<CreateTwilioConfigRequest>
{
    public CreateTwilioConfigRequestValidator()
    {
        RuleFor(x => x.IdAgent)
            .NotEmpty().WithMessage("ID do agente é obrigatório.");

        RuleFor(x => x.AccountSid)
            .NotEmpty().WithMessage("Account SID é obrigatório.")
            .Length(34).WithMessage("Account SID deve ter exatamente 34 caracteres.")
            .Matches("^AC[a-fA-F0-9]{32}$").WithMessage("Account SID deve ter o formato correto do Twilio.");

        RuleFor(x => x.AuthToken)
            .NotEmpty().WithMessage("Auth Token é obrigatório.")
            .Length(32).WithMessage("Auth Token deve ter exatamente 32 caracteres.")
            .Matches("^[a-fA-F0-9]{32}$").WithMessage("Auth Token deve conter apenas caracteres hexadecimais.");

        RuleFor(x => x.WebhookUrl)
            .NotEmpty().WithMessage("URL do Webhook é obrigatória.")
            .Matches(@"^(https?:\/\/)[\w\-\.]+(\.[\w\-]+)+.*$")
            .WithMessage("URL do Webhook deve ser uma URL válida (http ou https).")
            .MaximumLength(1000).WithMessage("URL do Webhook deve ter no máximo 1000 caracteres.");
    }
}
