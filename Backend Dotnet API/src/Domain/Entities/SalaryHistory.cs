
namespace Domain.Entities;

public class SalaryHistory : BaseEntity
{
    public string IdEmployee { get; set; }
    public Employee Employee { get; set; }
    public DateTime? ChangeDate { get; set; }
    public decimal NewSalary { get; set; }
    public string MotiveName { get; set; }
    public string? EmployeeCodSeniorNumCad { get; set; }
    public string? CompanyCodSeniorNumEmp { get; set; }
    public string? CompanyCodSeniorCodFil { get; set; }
    public string? MotiveCodSeniorCodMot { get; set; }
}
