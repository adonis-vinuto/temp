using System.Text.Json.Serialization;
using FluentValidation;

namespace Application.Handlers.File.DetachFromAgent;

public record DetachFromAgentFileRequest(
    Guid IdAgent
)
{
    [JsonIgnore]
    public Guid IdFile { get; set; }
}

public class DetachFromAgentFileRequestValidator : AbstractValidator<DetachFromAgentFileRequest>
{
    public DetachFromAgentFileRequestValidator()
    {
        RuleFor(x => x.IdFile)
            .NotEmpty().WithMessage("Informe o File a ser desanexado.");

        RuleFor(x => x.IdAgent)
            .NotEmpty().WithMessage("Informe o Agente a ser desanexado.");
    }
}
