using System;
using FluentValidation;
using System.Text.Json.Serialization;

namespace Application.Handlers.SeniorErpConfig.Edit;

public record EditSeniorErpConfigRequest(
    string? Username,
    string? Password,
    string? WsdlUrl
)
{
    [JsonIgnore]
    public Guid Id { get; set; }
};

public class EditSeniorErpConfigRequestValidator : AbstractValidator<EditSeniorErpConfigRequest>
{
    public EditSeniorErpConfigRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id é obrigatório.");

        When(x => x.Username is not null, () =>
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username é obrigatório.")
            .MaximumLength(100)
            .WithMessage("Username deve ter no máximo 100 caracteres."));

        When(x => x.Password is not null, () =>
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password é obrigatório.")
            .MaximumLength(100)
            .WithMessage("Password deve ter no máximo 100 caracteres."));

        When(x => x.WsdlUrl is not null, () =>
        RuleFor(x => x.WsdlUrl)
            .NotNull()
            .WithMessage("WsdlUrl é obrigatório."));
    }
}