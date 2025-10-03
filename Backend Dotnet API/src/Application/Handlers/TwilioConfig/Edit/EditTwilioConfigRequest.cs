using System.Text.Json.Serialization;
using FluentValidation;

namespace Application.Handlers.TwilioConfig.Edit;

public record EditTwilioConfigRequest(
    string? AccountSid,
    string? AuthToken,
    string? WebhookUrl
)
{
    [JsonIgnore] public Guid Id { get; set; }
};

public class EditTwilioConfigRequestValidator : AbstractValidator<EditTwilioConfigRequest>
{
    public EditTwilioConfigRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório.");

        When(x => x.AccountSid is not null, () =>
        RuleFor(x => x.AccountSid)
        .NotEmpty().WithMessage("Account SID é obrigatório.")
        .Length(34, 34).WithMessage("Account SID deve ter exatamente 34 caracteres.")
        .Matches("^AC[a-fA-F0-9]{32}$").WithMessage("Account SID deve ter o formato correto do Twilio."));

        When(x => x.AuthToken is not null, () =>
        RuleFor(x => x.AuthToken)
        .NotEmpty().WithMessage("Auth Token é obrigatório.")
        .Length(32, 32).WithMessage("Auth Token deve ter exatamente 32 caracteres.")
        .Matches("^[a-fA-F0-9]{32}$").WithMessage("Auth Token deve conter apenas caracteres hexadecimais."));

        When(x => x.WebhookUrl is not null, () =>
        RuleFor(x => x.WebhookUrl)
        .NotEmpty().WithMessage("URL do Webhook é obrigatório.")
        .Matches(@"^(https?:\/\/)[\w\-\.]+(\.[\w\-]+)+.*$").WithMessage("URL do Webhook deve ter o formato correto."));
    }
}
