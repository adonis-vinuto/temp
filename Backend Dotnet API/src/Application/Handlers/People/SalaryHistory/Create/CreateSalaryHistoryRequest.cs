using FluentValidation;

namespace Application.Handlers.People.SalaryHistory.Create;

public record CreateSalaryHistoryRequest(
    string IdEmployee,
    decimal NewSalary,
    string? EmployeeCodSeniorNumCad,
    string? CollaboratorTypeCodeSeniorTipcol,
    string? CompanyCodSeniorNumEmp,
    string? MotiveCodSeniorCodMot
);


public class CreateSalaryHistoryRequestValidator : AbstractValidator<CreateSalaryHistoryRequest>
{
    public CreateSalaryHistoryRequestValidator()
    {
        RuleFor(x => x.IdEmployee)
        .NotEmpty().WithMessage("O Id do trabalhador é obrigatório.");

        RuleFor(x => x.CollaboratorTypeCodeSeniorTipcol)
            .NotEmpty().WithMessage("O tipo do código do coloborador é necessário.")
            .Length(1, 400).WithMessage("tipo do código do coloborador deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(x => x.CompanyCodSeniorNumEmp)
            .NotEmpty().WithMessage("O código da compania é necessário.")
            .Length(1, 400).WithMessage("o código da compania deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(x => x.EmployeeCodSeniorNumCad)
            .NotEmpty().WithMessage("O código do trabalhador é necessário.")
            .Length(1, 400).WithMessage("o código do trabalhador deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(x => x.MotiveCodSeniorCodMot)
            .NotEmpty().WithMessage("O código do motivo é necessário.")
            .Length(1, 400).WithMessage("o código do motivo deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(x => x.NewSalary)
                .NotNull().WithMessage("O novo salário deve ser diferente de null.");
    }
}
