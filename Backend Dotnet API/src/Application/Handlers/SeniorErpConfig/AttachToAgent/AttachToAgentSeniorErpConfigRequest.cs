using System.Data;
using System.Text.Json.Serialization;
using Domain.Enums;
using FluentValidation;

namespace Application.Handlers.SeniorErpConfig.AttachToAgent;

public record AttachToAgentSeniorErpConfigRequest(
    Guid IdAgent
)
{
    [JsonIgnore]
    public Guid Id { get; set; }
}


public class AttachToAgentSeniorErpConfigRequestValidator : AbstractValidator<AttachToAgentSeniorErpConfigRequest>
{
    public AttachToAgentSeniorErpConfigRequestValidator()
    {
        RuleFor(x => x.Id)
        .NotEmpty().WithMessage("Informe a configuração do ERP Senior a ser anexado.");

        RuleFor(x => x.IdAgent)
            .NotEmpty().WithMessage("Informe o Agente a ser anexado.");
    }
}
