using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using FluentValidation;
using System.Text.Json.Serialization;

namespace Application.Handlers.People.Payroll.Edit;

public record EditPayrollRequest(
    string? IdEmployee,
    string? PayrollPeriodCod,
    string? EventName,
    decimal? EventAmount,
    string? EventTypeName,
    DateTime? ReferenceDate,
    string? CalculationTypeName,
    string? EmployeeCodSeniorNumCad,
    string? CollaboratorTypeCodeSeniorTipCol,
    string? CompanyCodSeniorNumEmp,
    string? PayrollPeriodCodSeniorCodCal,
    string? EventTypeCodSeniorTipEve,
    string? CalculationTypeCodSeniorTipCal
)
{
    [JsonIgnore] public Guid Id { get; set; }

}
public class EditPayrollRequestValidator : AbstractValidator<EditPayrollRequest>
{
    public EditPayrollRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Informe o Payroll a ser editado");

        When(x => x.IdEmployee is not null, () =>
        RuleFor(x => x.IdEmployee)
            .NotEmpty().WithMessage("Informe o Employee relacionado ao Payroll"));

        When(x => x.EventName is not null, () =>
        RuleFor(x => x.EventName)
            .NotEmpty().WithMessage("EventName é necessário.")
            .MaximumLength(100).WithMessage("EventName deve conter no máximo {MaxLength} caracteres."));

        When(x => x.EventAmount != 0, () =>
        RuleFor(x => x.EventAmount)
            .GreaterThan(0).WithMessage("EventAmount deve ser maior que zero."));

        When(x => x.PayrollPeriodCod is not null, () =>
        RuleFor(x => x.PayrollPeriodCod)
            .MaximumLength(50).WithMessage("PayrollPeriodCod deve conter no máximo {MaxLength} caracteres"));

        When(x => x.EventTypeName is not null, () =>
        RuleFor(x => x.EventTypeName)
            .MaximumLength(100).WithMessage("EventTypeName não deve exceder {MaxLength} caracteres."));

        When(x => x.ReferenceDate.HasValue, () =>
        RuleFor(x => x.ReferenceDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("ReferenceDate não pode ser uma data futura."));

        When(x => x.CalculationTypeName is not null, () =>
        RuleFor(x => x.CalculationTypeName)
            .MaximumLength(100).WithMessage("CalculationTypeName não deve exceder {MaxLength} caracteres."));

        When(x => x.EmployeeCodSeniorNumCad is not null, () =>
        RuleFor(x => x.EmployeeCodSeniorNumCad)
            .MaximumLength(50).WithMessage("EmployeeCodSeniorNumCad não deve exceder {MaxLength} caracteres."));

        When(x => x.CollaboratorTypeCodeSeniorTipCol is not null, () =>
        RuleFor(x => x.CollaboratorTypeCodeSeniorTipCol)
            .MaximumLength(50).WithMessage("CollaboratorTypeCodeSeniorTipCol não deve exceder {MaxLength} caracteres."));

        When(x => x.CompanyCodSeniorNumEmp is not null, () =>
        RuleFor(x => x.CompanyCodSeniorNumEmp)
            .MaximumLength(50).WithMessage("CompanyCodSeniorNumEmp não deve exceder {MaxLength} caracteres."));

        When(x => x.PayrollPeriodCodSeniorCodCal is not null, () =>
        RuleFor(x => x.PayrollPeriodCodSeniorCodCal)
            .MaximumLength(50).WithMessage("PayrollPeriodCodSeniorCodCal não deve exceder {MaxLength} caracteres."));

        When(x => x.EventTypeCodSeniorTipEve is not null, () =>
        RuleFor(x => x.EventTypeCodSeniorTipEve)
            .MaximumLength(50).WithMessage("EventTypeCodSeniorTipEve não deve exceder {MaxLength} caracteres."));

        When(x => x.CalculationTypeCodSeniorTipCal is not null, () =>
        RuleFor(x => x.CalculationTypeCodSeniorTipCal)
            .MaximumLength(50).WithMessage("CalculationTypeCodSeniorTipCal não deve exceder {MaxLength} caracteres."));
    }
}
