using FluentValidation;

namespace Application.Handlers.People.SalaryHistory.Edit;

public record EditSalaryHistoryRequest(
    string? IdEmployee,
    decimal? NewSalary,
    string? EmployeeCodSeniorNumCad,
    string? CollaboratorTypeCodeSeniorTipcol,
    string? CompanyCodSeniorNumEmp,
    string? MotiveCodSeniorCodMot
);

public class EditAgentRequestValidator : AbstractValidator<EditSalaryHistoryRequest>
{
    public EditAgentRequestValidator()
    {
        When(x => x.IdEmployee is not null, () =>
            RuleFor(x => x.IdEmployee)
            .NotEmpty().WithMessage("O Id do colaborador é obrigatório."));

        When(x => x.CollaboratorTypeCodeSeniorTipcol is not null, () =>
            RuleFor(x => x.CollaboratorTypeCodeSeniorTipcol)
                .NotEmpty().WithMessage("O tipo do código do coloborador é necessário.")
                .Length(1, 400).WithMessage("tipo do código do coloborador deve conter entre {MinLength} e {MaxLength} caracteres.")
            );

        When(x => x.CompanyCodSeniorNumEmp is not null, () =>
            RuleFor(x => x.CompanyCodSeniorNumEmp)
                .NotEmpty().WithMessage("O código da compania é necessário.")
                .Length(1, 400).WithMessage("o código da compania deve conter entre {MinLength} e {MaxLength} caracteres.")
            );

        When(x => x.EmployeeCodSeniorNumCad is not null, () =>
            RuleFor(x => x.EmployeeCodSeniorNumCad)
                .NotEmpty().WithMessage("O código do coloborador é necessário.")
                .Length(1, 400).WithMessage("o código do coloborador deve conter entre {MinLength} e {MaxLength} caracteres.")
        );

        When(x => x.MotiveCodSeniorCodMot is not null, () =>
            RuleFor(x => x.MotiveCodSeniorCodMot)
                .NotEmpty().WithMessage("O código do motivo é necessário.")
                .Length(1, 400).WithMessage("o código do motivo deve conter entre {MinLength} e {MaxLength} caracteres.")
        );

        When(x => x.NewSalary is not null, () =>
        RuleFor(x => x.NewSalary)
                .NotNull().WithMessage("O novo salário deve ser diferente de null.")
        );

    }
}