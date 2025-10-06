namespace Application.DTOs.SalaryHistory;
public class SalaryHistoryResponse
{
    public Guid Id { get; set; }
    public Guid IdEmployee { get; set; }
    public DateTime? ChangeDate { get; set; }
    public decimal NewSalary { get; set; }
    public string? EmployeeCodSeniorNumCad { get; set; }
    public string? CollaboratorTypeCodeSeniorTipcol { get; set; }
    public string? CompanyCodSeniorNumEmp { get; set; }
    public string? MotiveCodSeniorCodMot { get; set; }
}
