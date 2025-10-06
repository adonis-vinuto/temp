using Domain.Enums;
using Domain.Helpers;

namespace Domain.Entities;

public class Employee
{
    public string Id { get; set; }
    public Guid IdKnowledge { get; set; }
    public Knowledge Knowledge { get; set; }
    public string? CompanyName { get; set; }
    public string? FullName { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public string? StatusDescription { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? CostCneterName { get; set; }
    public decimal? Salary { get; set; }
    public decimal? ComplementarySalary { get; set; }
    public DateTime? SalaryEffectiveDate { get; set; }
    public Gender Gender { get; set; }
    public string? StreetAddress { get; set; }
    public string? AddressNumber { get; set; }
    public string? CityName { get; set; }
    public string? Race { get; set; }
    public string? PostalCode { get; set; }
    public string? CompanyCodSeniorNumEmp { get; set; }
    public string? EmployeeCodSeniorNumCad { get; set; }
    public string? CollaboratorTypeCodeSeniorTipeCol { get; set; }
    public string? StatusCodSenior { get; set; }
    public string? CostCenterCodSeniorCodCcu { get; set; }
    public IEnumerable<SalaryHistory> SalaryHistories => _salaryHistories;
    private readonly List<SalaryHistory> _salaryHistories = [];
    public IEnumerable<Payroll> Payrolls => _payrolls;
    private readonly List<Payroll> _payrolls = [];
    public DateTime CreatedAt { get; set; } = DateTimeProvider.DataHoraAtual();
}

