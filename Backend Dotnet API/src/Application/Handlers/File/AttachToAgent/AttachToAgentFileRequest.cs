using System.Data;
using System.Text.Json.Serialization;
using Domain.Enums;
using FluentValidation;

namespace Application.Handlers.File.AttachToAgent;

public record AttachToAgentFileRequest(
    Guid IdAgent
)
{
    [JsonIgnore]
    public Guid IdFile { get; set; }
}


public class AttachToAgentFileRequestValidator : AbstractValidator<AttachToAgentFileRequest>
{
    public AttachToAgentFileRequestValidator()
    {
        RuleFor(x => x.IdFile)
        .NotEmpty().WithMessage("Informe o File a ser anexado.");

        RuleFor(x => x.IdAgent)
            .NotEmpty().WithMessage("Informe o Agente a ser anexado.");


    }
}
