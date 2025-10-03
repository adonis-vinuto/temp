using System.Data;
using System.Text.Json.Serialization;
using Domain.Enums;
using FluentValidation;

namespace Application.Handlers.Knowledge.AttachToAgent;

public record AttachToAgentKnowledgeRequest(
    Guid IdAgent
)
{
    [JsonIgnore]
    public Guid Id { get; set; }
}


public class AttachToAgentKnowledgeRequestValidator : AbstractValidator<AttachToAgentKnowledgeRequest>
{
    public AttachToAgentKnowledgeRequestValidator()
    {
        RuleFor(x => x.Id)
        .NotEmpty().WithMessage("Informe o Knowledge a ser anexado.");

        RuleFor(x => x.IdAgent)
            .NotEmpty().WithMessage("Informe o Agente a ser anexado.");
    }
}
