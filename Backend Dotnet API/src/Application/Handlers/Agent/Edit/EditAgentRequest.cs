using System.Text.Json.Serialization;
using FluentValidation;

namespace Application.Handlers.Agent.Edit;

public record EditAgentRequest(
    string? Name,
    string? Description
)
{
    [JsonIgnore] public Guid Id { get; set; }
}

public class EditAgentRequestValidator : AbstractValidator<EditAgentRequest>
{
    public EditAgentRequestValidator()
    {

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id é necessário.");

        When(x => x.Name is not null, () =>

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é necessário.")
                .Length(1, 2500).WithMessage("Nome deve ter entre {MinLength} e {MaxLength} caracteres.")
        );

        When(x => x.Description is not null, () =>

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Descrição é necessária.")
                .Length(1, 2500).WithMessage("Descrição deve ter entre {MinLength} e {MaxLength} caracteres.")
        );
        
    }
}