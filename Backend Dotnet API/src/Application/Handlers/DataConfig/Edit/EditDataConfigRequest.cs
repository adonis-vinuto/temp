using System.Text.Json.Serialization;
using FluentValidation;

namespace Application.Handlers.DataConfig.Edit;

public record EditDataConfigRequest(
    string? SqlHost,
    string? SqlPort,
    string? SqlUser,
    string? SqlPassword,
    string? SqlDatabase,
    string? BlobConnectionString,
    string? BlobContainerName
)
{
    [JsonIgnore] public Guid Id { get; set; }
};

public class EditDataConfigRequestValidator : AbstractValidator<EditDataConfigRequest>
{
    public EditDataConfigRequestValidator()
    {
        
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Data config's ID é necessário.");

        When(x => x.SqlHost is not null, () => RuleFor(x => x.SqlHost)
            .NotEmpty().WithMessage("SQL Host é necessário.")
            .Length(1, 100).WithMessage("SQL Host deve conter entre {MinLength} e {MaxLength} caracteres."));
        
        When(x => x.SqlPort is not null, () => RuleFor(x => x.SqlPort)
            .NotEmpty().WithMessage("SQL Port é necessário.")
            .Length(1, 10).WithMessage("SQL Port deve conter entre {MinLength} e {MaxLength} caracteres."));

        When(x => x.SqlUser is not null, () => RuleFor(x => x.SqlUser)
            .NotEmpty().WithMessage("SQL User é necessário.")
            .Length(1, 50).WithMessage("SQL User deve conter entre {MinLength} e {MaxLength} caracteres."));

        When(x => x.SqlPassword is not null, () => RuleFor(x => x.SqlPassword)
            .NotEmpty().WithMessage("SQL Password é necessário.")
            .Length(1, 100).WithMessage("SQL Password deve conter entre {MinLength} e {MaxLength} caracteres."));

        When(x => x.SqlDatabase is not null, () => RuleFor(x => x.SqlDatabase)
            .NotEmpty().WithMessage("SQL Database é necessário.")
            .Length(1, 100).WithMessage("SQL Database deve conter entre {MinLength} e {MaxLength} caracteres."));

        When(x => x.BlobConnectionString is not null, () => RuleFor(x => x.BlobConnectionString)
            .NotEmpty().WithMessage("Blob Connection String é necessário.")
            .Length(1, 100).WithMessage("Blob Connection String deve conter entre {MinLength} e {MaxLength} caracteres."));

        When(x => x.BlobContainerName is not null, () => RuleFor(x => x.BlobContainerName)
            .NotEmpty().WithMessage("Blob Container Name é necessário.")
            .Length(1, 100).WithMessage("Blob Container Name deve conter entre {MinLength} e {MaxLength} caracteres."));    
    }
}