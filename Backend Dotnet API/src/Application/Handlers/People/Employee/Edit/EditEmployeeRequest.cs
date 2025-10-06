using System.Text.Json.Serialization;
using FluentValidation;
using Domain.Enums;

namespace Application.Handlers.People.Employee.Edit;

public record EditEmployeeRequest(
    string? CompanyName,
    string? FullName,
    DateTime? AdmissionDate,
    DateTime? TerminationDate,
    string? StatusDescription,
    DateTime? BirthDate,
    string? CostCneterName,
    decimal? Salary,
    decimal? ComplementarySalary,
    DateTime? SalaryEffectiveDate,
    Gender? Gender,
    string? StreetAddress,
    string? AddressNumber,
    string? CityName,
    string? Race,
    string? PostalCode,
    string? CompanyCodSeniorNumemp,
    string? EmployeeCodSeniorNumcad,
    string? CollaboratorTypeCodeSeniorTipcol,
    string? StatusCodSeniorSitafa,
    string? CostCenterCodSeniorCodccu
)
{
    [JsonIgnore] public string Id { get; set; }
}

public class EditEmployeeRequestValidator : AbstractValidator<EditEmployeeRequest>
{
    public EditEmployeeRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório.");

        When(x => x.CompanyName is not null, () =>
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("CompanyName é obrigatório.")
                .MaximumLength(200).WithMessage("CompanyName deve ter no máximo 200 caracteres.")
        );

        When(x => x.FullName is not null, () =>
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName é obrigatório.")
                .MaximumLength(200).WithMessage("FullName deve ter no máximo 200 caracteres.")
        );

        When(x => x.AdmissionDate != default, () =>
            RuleFor(x => x.AdmissionDate)
                .NotEmpty().WithMessage("AdmissionDate é obrigatório.")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("AdmissionDate não pode ser no futuro.")
        );

        When(x => x.TerminationDate != default, () =>
            RuleFor(x => x.TerminationDate)
                .GreaterThan(x => x.AdmissionDate)
                .WithMessage("TerminationDate deve ser maior que AdmissionDate.")
        );

        When(x => x.StatusDescription is not null, () =>
            RuleFor(x => x.StatusDescription)
                .NotEmpty().WithMessage("StatusDescription é obrigatório.")
                .MaximumLength(100).WithMessage("StatusDescription deve ter no máximo 100 caracteres.")
        );

        When(x => x.BirthDate != default, () =>
            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("BirthDate é obrigatório.")
                .LessThan(DateTime.Today).WithMessage("BirthDate deve ser anterior à data atual.")
        );

        When(x => x.CostCneterName is not null, () =>
            RuleFor(x => x.CostCneterName)
                .NotEmpty().WithMessage("CostCenterName é obrigatório.")
                .MaximumLength(150).WithMessage("CostCenterName deve ter no máximo 150 caracteres.")
        );

        When(x => x.Salary != default, () =>
            RuleFor(x => x.Salary)
                .GreaterThanOrEqualTo(0).WithMessage("Salary deve ser maior ou igual a 0.")
        );

        When(x => x.ComplementarySalary != default, () =>
            RuleFor(x => x.ComplementarySalary)
                .GreaterThanOrEqualTo(0).WithMessage("ComplementarySalary deve ser maior ou igual a 0.")
        );

        When(x => x.SalaryEffectiveDate != default, () =>
            RuleFor(x => x.SalaryEffectiveDate)
                .NotEmpty().WithMessage("SalaryEffectiveDate é obrigatório.")
                .GreaterThanOrEqualTo(x => x.AdmissionDate).WithMessage("SalaryEffectiveDate deve ser maior ou igual à AdmissionDate.")
        );

        When(x => x.Gender != default, () =>
            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender é obrigatório.")
                .IsInEnum().WithMessage("Gender deve ser um valor válido 0 ou 1.")
        );

        When(x => x.StreetAddress is not null, () =>
            RuleFor(x => x.StreetAddress)
                .NotEmpty().WithMessage("StreetAddress é obrigatório.")
                .MaximumLength(250).WithMessage("StreetAddress deve ter no máximo 250 caracteres.")
        );

        When(x => x.AddressNumber is not null, () =>
            RuleFor(x => x.AddressNumber)
                .NotEmpty().WithMessage("AddressNumber é obrigatório.")
                .MaximumLength(20).WithMessage("AddressNumber deve ter no máximo 20 caracteres.")
        );

        When(x => x.CityName is not null, () =>
            RuleFor(x => x.CityName)
                .NotEmpty().WithMessage("CityName é obrigatório.")
                .MaximumLength(150).WithMessage("CityName deve ter no máximo 150 caracteres.")
        );

        When(x => x.Race is not null, () =>
            RuleFor(x => x.Race)
                .MaximumLength(50).WithMessage("Race deve ter no máximo 50 caracteres.")
        );

        When(x => x.PostalCode is not null, () =>
            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("PostalCode é obrigatório.")
                .Matches(@"^\d{5}-\d{3}$").WithMessage("PostalCode deve estar no formato 00000-000.")
        );

        When(x => x.CompanyCodSeniorNumemp is not null, () =>
            RuleFor(x => x.CompanyCodSeniorNumemp)
                .NotEmpty().WithMessage("CompanyCodSeniorNumemp é obrigatório.")
                .MaximumLength(50).WithMessage("CompanyCodSeniorNumemp deve ter no máximo 50 caracteres.")
        );

        When(x => x.EmployeeCodSeniorNumcad is not null, () =>
            RuleFor(x => x.EmployeeCodSeniorNumcad)
                .NotEmpty().WithMessage("EmployeeCodSeniorNumcad é obrigatório.")
                .MaximumLength(50).WithMessage("EmployeeCodSeniorNumcad deve ter no máximo 50 caracteres.")
        );

        When(x => x.CollaboratorTypeCodeSeniorTipcol is not null, () =>
            RuleFor(x => x.CollaboratorTypeCodeSeniorTipcol)
                .NotEmpty().WithMessage("CollaboratorTypeCodeSeniorTipcol é obrigatório.")
                .MaximumLength(50).WithMessage("CollaboratorTypeCodeSeniorTipcol deve ter no máximo 50 caracteres.")
        );

        When(x => x.StatusCodSeniorSitafa is not null, () =>
            RuleFor(x => x.StatusCodSeniorSitafa)
                .NotEmpty().WithMessage("StatusCodSeniorSitafa é obrigatório.")
                .MaximumLength(50).WithMessage("StatusCodSeniorSitafa deve ter no máximo 50 caracteres.")
        );

        When(x => x.CostCenterCodSeniorCodccu is not null, () =>
            RuleFor(x => x.CostCenterCodSeniorCodccu)
                .NotEmpty().WithMessage("CostCenterCodSeniorCodccu é obrigatório.")
                .MaximumLength(50).WithMessage("CostCenterCodSeniorCodccu deve ter no máximo 50 caracteres.")
        );
    }
}
