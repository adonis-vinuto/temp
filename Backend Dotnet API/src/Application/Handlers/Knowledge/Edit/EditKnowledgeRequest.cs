using System.Data;
using System.Text.Json.Serialization;
using Domain.Enums;
using FluentValidation;

namespace Application.Handlers.Knowledge.Edit;

public record EditKnowledgeRequest(
    string? Name,
    string? Description
)
{
    [JsonIgnore] public Guid Id { get; set; }
};

public class EditKnowledgeRequestValidator : AbstractValidator<EditKnowledgeRequest>
{
    public EditKnowledgeRequestValidator()
    {
        RuleFor(x => x.Id)
        .NotEmpty().WithMessage("Informe o Knowledge a ser editado");

        When(x => x.Name != null, () =>
            RuleFor(x => x.Name)
            .Length(1, 100).WithMessage("Nome deve conter entre {MinLength} e {MaxLength} caracteres.")
        );

        When(x => x.Description != null, () =>
            RuleFor(x => x.Description)
            .Length(1, 250).WithMessage("Descrição deve conter entre {MinLength} e {MaxLength} caracteres.")
        );

    }
}
