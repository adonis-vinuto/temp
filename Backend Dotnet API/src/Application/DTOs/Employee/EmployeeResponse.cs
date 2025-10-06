using Domain.Enums;

namespace Application.DTOs.Employee;

public class EmployeeResponse
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; }
    public string FullName { get; set; }
    public DateTime AdmissionDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public string StatusDescription { get; set; }
    public DateTime BirthDate { get; set; }
    public string CostCenterName { get; set; }
    public decimal Salary { get; set; }
    public decimal ComplementarySalary { get; set; }
    public DateTime SalaryEffectiveDate { get; set; }
    public Gender Gender { get; set; }
    public string StreetAddress { get; set; }
    public string AddressNumber { get; set; }
    public string CityName { get; set; }
    public string Race { get; set; }
    public string PostalCode { get; set; }
    public string CompanyCodSeniorNumemp { get; set; }
    public string EmployeeCodSeniorNumcad { get; set; }
    public string CollaboratorTypeCodeSeniorTipcol { get; set; }
    public string StatusCodSeniorSitafa { get; set; }
    public string CostCenterCodSeniorCodccu { get; set; }
}