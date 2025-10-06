using Domain.Enums;
using FluentValidation;

namespace Application.Handlers.Agent.Create;

public record CreateAgentRequest(
    string Name,
    string Description,
    TypeAgent Type
);


public class CreateAgentRequestValidator : AbstractValidator<CreateAgentRequest>
{
    public CreateAgentRequestValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Tipo deve ser People, Finance, Sales, Tax ou Support.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é necessário.")
            .Length(1, 100).WithMessage("Nome deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Descrição é necessária.")
            .Length(1, 250).WithMessage("Descrição deve conter entre {MinLength} e {MaxLength} caracteres.");
   
    }
}
