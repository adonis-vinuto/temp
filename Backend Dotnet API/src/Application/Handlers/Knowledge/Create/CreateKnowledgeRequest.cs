using Domain.Enums;
using FluentValidation;

namespace Application.Handlers.Knowledge.Create;

public record CreateKnowledgeRequest(
    string Name,
    string? Description,
    Origin? Origin
);


public class CreateKnowledgeRequestValidator : AbstractValidator<CreateKnowledgeRequest>
{
    public CreateKnowledgeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é necessário.")
            .Length(1, 100).WithMessage("Nome deve conter entre {MinLength} e {MaxLength} caracteres.");

        When(x => x.Description != null, () =>
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Descrição é necessária.")
                .Length(1, 250).WithMessage("Descrição deve conter entre {MinLength} e {MaxLength} caracteres.")
        );

        When(x => x.Origin != null, () =>
            RuleFor(x => x.Origin)
                .IsInEnum().WithMessage("Origem inválida")
        );
    }
}
