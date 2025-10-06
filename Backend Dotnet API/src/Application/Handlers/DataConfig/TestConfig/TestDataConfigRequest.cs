using Domain.Enums;
using FluentValidation;

namespace Application.Handlers.DataConfig.TestConfig;

public record TestDataConfigRequest(
    string Host,
    string Port,
    string User,
    string Password,
    string Database
);

public class TestDataConfigRequestValidator : AbstractValidator<TestDataConfigRequest>
{
    public TestDataConfigRequestValidator()
    {

        RuleFor(x => x.Host)
            .NotEmpty().WithMessage("Host é necessário.")
            .Length(1, 100).WithMessage("Host deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(x => x.Port)
            .NotEmpty().WithMessage("Port é necessário.")
            .Matches(@"^\d{1,5}$").WithMessage("Port deve ser um número entre 1 e 65535.");

        RuleFor(x => x.User)
            .NotEmpty().WithMessage("User é necessário.")
            .Length(1, 100).WithMessage("User deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password é necessário.")
            .Length(1, 100).WithMessage("Password deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(x => x.Database)
            .NotEmpty().WithMessage("Database é necessário.")
            .Length(1, 100).WithMessage("Database deve conter entre {MinLength} e {MaxLength} caracteres.");
    }
}