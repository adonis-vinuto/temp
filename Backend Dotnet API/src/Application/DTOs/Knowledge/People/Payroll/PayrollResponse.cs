using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.Knowledge.People.Payroll;

public class PayrollResponse
{
    public Guid Id { get; set; }
    public Guid IdEmployee { get; set; }
    public string? PayrollPeriodCod { get; set; }
    public string EventName { get; set; } = null!;
    public decimal EventAmount { get; set; }
    public string? EventTypeName { get; set; }
    public DateTime? ReferenceDate { get; set; }
    public string? CalculationTypeName { get; set; }
    public string? EmployeeCodSeniorNumCad { get; set; }
    public string? CollaboratorTypeCodeSeniorTipCol { get; set; }
    public string? CompanyCodSeniorNumEmp { get; set; }
    public string? PayrollPeriodCodSeniorCodCal { get; set; }
    public string? EventTypeCodSeniorTipEve { get; set; }
    public string? CalculationTypeCodSeniorTipCal { get; set; }
    
}
