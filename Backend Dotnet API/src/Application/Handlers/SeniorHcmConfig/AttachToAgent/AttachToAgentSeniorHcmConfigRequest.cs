using System.Data;
using System.Text.Json.Serialization;
using Domain.Enums;
using FluentValidation;

namespace Application.Handlers.SeniorHcmConfig.AttachToAgent;

public record AttachToAgentSeniorHcmConfigRequest(
    Guid IdAgent
)
{
    [JsonIgnore]
    public Guid Id { get; set; }
}


public class AttachToAgentSeniorHcmConfigRequestValidator : AbstractValidator<AttachToAgentSeniorHcmConfigRequest>
{
    public AttachToAgentSeniorHcmConfigRequestValidator()
    {
        RuleFor(x => x.Id)
        .NotEmpty().WithMessage("Informe a configuração do ERP Senior a ser anexado.");

        RuleFor(x => x.IdAgent)
            .NotEmpty().WithMessage("Informe o Agente a ser anexado.");
    }
}
