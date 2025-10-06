

namespace Domain.Entities;

public class Payroll : BaseEntity
{
    public string IdEmployee { get; set; }
    public Employee Employee { get; set; }
    public string? PayrollPeriodCod { get; set; }
    public string? EventName { get; set; }
    public decimal? EventAmount { get; set; }
    public string? EventTypeName { get; set; }
    public DateTime? ReferenceDate { get; set; }
    public string? CalculationTypeName { get; set; }
    public string? EmployeeCodSeniorNumCad { get; set; }
    public string? CollaboratorTypeCodeSeniorTipCol { get; set; }
    public string? CompanyCodSeniorNumEmp { get; set; }
    public string? PayrollPeriodCodSeniorCodCal { get; set; }
    public string? EventTypeCodSeniorTipEve { get; set; }
    public string? EventCodSeniorCodenv { get; set; }
    public string? CalculationTypeCodSeniorTipCal { get; set; }
}
