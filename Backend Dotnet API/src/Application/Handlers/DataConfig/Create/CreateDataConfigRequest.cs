using Domain.Enums;
using FluentValidation;

namespace Application.Handlers.DataConfig.Create;

public record CreateDataConfigRequest(
    string SqlHost,
    string SqlPort,
    string SqlUser,
    string SqlPassword,
    string SqlDatabase,
    string? BlobConnectionString,
    string? BlobContainerName
);

public class CreateDataConfigRequestValidator : AbstractValidator<CreateDataConfigRequest>
{
    public CreateDataConfigRequestValidator()
    {

        RuleFor(x => x.SqlHost)
            .NotEmpty().WithMessage("SQL Host é necessário.")
            .Length(1, 100).WithMessage("SQL Host deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(x => x.SqlPort)
            .NotEmpty().WithMessage("SQL Port é necessário.")
            .Matches("^[0-9]+$").WithMessage("SQL Port deve conter apenas números.")
            .Length(1, 100).WithMessage("SQL Port deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(x => x.SqlUser)
            .NotEmpty().WithMessage("SQL User é necessário.")
            .Length(1, 50).WithMessage("SQL User deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(x => x.SqlPassword)
            .NotEmpty().WithMessage("SQL Password é necessário.")
            .Length(1, 100).WithMessage("SQL Password deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(x => x.SqlDatabase)
            .NotEmpty().WithMessage("SQL Database é necessário.")
            .Length(1, 100).WithMessage("SQL Database deve conter entre {MinLength} e {MaxLength} caracteres.");

        When(x => x.BlobConnectionString is not null, () => RuleFor(x => x.BlobConnectionString)
               .NotEmpty().WithMessage("Blob Connection String é necessário.")
               .Length(1, 100).WithMessage("Blob Connection String deve conter entre {MinLength} e {MaxLength} caracteres."));

        When(x => x.BlobContainerName is not null, () => RuleFor(x => x.BlobContainerName)
               .NotEmpty().WithMessage("Blob Container Name é necessário.")
               .Length(1, 100).WithMessage("Blob Container Name deve conter entre {MinLength} e {MaxLength} caracteres."));

        When(x => x.BlobConnectionString is not null, () => RuleFor(x => x.BlobContainerName)
            .NotEmpty().WithMessage("Blob Container Name é necessário.")
            .Length(1, 100).WithMessage("Blob Container Name deve conter entre {MinLength} e {MaxLength} caracteres."));

        When(x => x.BlobContainerName is not null, () => RuleFor(x => x.BlobConnectionString)
            .NotEmpty().WithMessage("Blob Connection String é necessário.")
            .Length(1, 100).WithMessage("Blob Connection String deve conter entre {MinLength} e {MaxLength} caracteres."));
    }
}