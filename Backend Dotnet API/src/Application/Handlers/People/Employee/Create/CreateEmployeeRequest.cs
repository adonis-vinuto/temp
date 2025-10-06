using System.Text.Json.Serialization;
using FluentValidation;
using Domain.Enums;

namespace Application.Handlers.People.Employee.Create;

public record CreateEmployeeRequest(
    string CompanyName,
    string FullName,
    DateTime AdmissionDate,
    DateTime? TerminationDate,
    string StatusDescription,
    DateTime BirthDate,
    string CostCneterName,
    decimal Salary,
    decimal ComplementarySalary,
    DateTime SalaryEffectiveDate,
    Gender Gender,
    string StreetAddress,
    string AddressNumber,
    string CityName,
    string Race,
    string PostalCode,
    string CompanyCodSeniorNumemp,
    string EmployeeCodSeniorNumcad,
    string CollaboratorTypeCodeSeniorTipcol,
    string StatusCodSeniorSitafa,
    string CostCenterCodSeniorCodccu
)
{
    [JsonIgnore] public Guid IdKnowledge { get; set; }
};

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(x => x.IdKnowledge)
            .NotEmpty().WithMessage("IdKnowledge é obrigatório.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("CompanyName é obrigatório.")
            .MaximumLength(200).WithMessage("CompanyName deve ter no máximo 200 caracteres.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName é obrigatório.")
            .MaximumLength(200).WithMessage("FullName deve ter no máximo 200 caracteres.");

        RuleFor(x => x.AdmissionDate)
            .NotEmpty().WithMessage("AdmissionDate é obrigatório.")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("AdmissionDate não pode ser no futuro.");

        RuleFor(x => x.TerminationDate)
            .GreaterThan(x => x.AdmissionDate)
            .When(x => x.TerminationDate.HasValue)
            .WithMessage("TerminationDate deve ser maior que AdmissionDate.");

        RuleFor(x => x.StatusDescription)
            .NotEmpty().WithMessage("StatusDescription é obrigatório.")
            .MaximumLength(100).WithMessage("StatusDescription deve ter no máximo 100 caracteres.");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("BirthDate é obrigatório.")
            .LessThan(DateTime.Today).WithMessage("BirthDate deve ser anterior à data atual.");

        RuleFor(x => x.CostCneterName)
            .NotEmpty().WithMessage("CostCenterName é obrigatório.")
            .MaximumLength(150).WithMessage("CostCenterName deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Salary)
            .GreaterThanOrEqualTo(0).WithMessage("Salary deve ser maior ou igual a 0.");

        RuleFor(x => x.ComplementarySalary)
            .GreaterThanOrEqualTo(0).WithMessage("ComplementarySalary deve ser maior ou igual a 0.");

        RuleFor(x => x.SalaryEffectiveDate)
            .NotEmpty().WithMessage("SalaryEffectiveDate é obrigatório.")
            .GreaterThanOrEqualTo(x => x.AdmissionDate).WithMessage("SalaryEffectiveDate deve ser maior ou igual à AdmissionDate.");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender é obrigatório.")
            .IsInEnum().WithMessage("Gender deve ser um valor válido 0 ou 1.");

        RuleFor(x => x.StreetAddress)
            .NotEmpty().WithMessage("StreetAddress é obrigatório.")
            .MaximumLength(250).WithMessage("StreetAddress deve ter no máximo 250 caracteres.");

        RuleFor(x => x.AddressNumber)
            .NotEmpty().WithMessage("AddressNumber é obrigatório.")
            .MaximumLength(20).WithMessage("AddressNumber deve ter no máximo 20 caracteres.");

        RuleFor(x => x.CityName)
            .NotEmpty().WithMessage("CityName é obrigatório.")
            .MaximumLength(150).WithMessage("CityName deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Race)
            .MaximumLength(50).WithMessage("Race deve ter no máximo 50 caracteres.");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("PostalCode é obrigatório.")
            .Matches(@"^\d{5}-\d{3}$").WithMessage("PostalCode deve estar no formato 00000-000.");

        RuleFor(x => x.CompanyCodSeniorNumemp)
            .NotEmpty().WithMessage("CompanyCodSeniorNumemp é obrigatório.")
            .MaximumLength(50).WithMessage("CompanyCodSeniorNumemp deve ter no máximo 50 caracteres.");

        RuleFor(x => x.EmployeeCodSeniorNumcad)
            .NotEmpty().WithMessage("EmployeeCodSeniorNumcad é obrigatório.")
            .MaximumLength(50).WithMessage("EmployeeCodSeniorNumcad deve ter no máximo 50 caracteres.");

        RuleFor(x => x.CollaboratorTypeCodeSeniorTipcol)
            .NotEmpty().WithMessage("CollaboratorTypeCodeSeniorTipcol é obrigatório.")
            .MaximumLength(50).WithMessage("CollaboratorTypeCodeSeniorTipcol deve ter no máximo 50 caracteres.");

        RuleFor(x => x.StatusCodSeniorSitafa)
            .NotEmpty().WithMessage("StatusCodSeniorSitafa é obrigatório.")
            .MaximumLength(50).WithMessage("StatusCodSeniorSitafa deve ter no máximo 50 caracteres.");

        RuleFor(x => x.CostCenterCodSeniorCodccu)
            .NotEmpty().WithMessage("CostCenterCodSeniorCodccu é obrigatório.")
            .MaximumLength(50).WithMessage("CostCenterCodSeniorCodccu deve ter no máximo 50 caracteres.");
    }
}
