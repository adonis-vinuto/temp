using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Knowledge.People.Payroll;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.People.Payroll.Create;

public class CreatePayrollHandler : BaseHandler
{
    private readonly IPayrollRepository _payrollRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IEmployeeRepository _employeeRepository;


    public CreatePayrollHandler(
        IPayrollRepository payrollRepository,
        IAuthenticationService authenticationService,
        IEmployeeRepository employeeRepository)
    {
        _payrollRepository = payrollRepository;
        _authenticationService = authenticationService;
        _employeeRepository = employeeRepository;
    }

    public async Task<ErrorOr<PayrollResponse>> Handle(CreatePayrollRequest request, CancellationToken cancellationToken)
    {
        if (Validate(request, new CreatePayrollRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        Domain.Entities.Employee? employee = await _employeeRepository.SearchByIdEmployeeAsync(request.IdEmployee, cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.EmployeeNotFound;
        }

        var payroll = new Domain.Entities.Payroll
        {
            IdEmployee = request.IdEmployee,
            PayrollPeriodCod = request.PayrollPeriodCod,
            EventName = request.EventName,
            EventAmount = request.EventAmount,
            EventTypeName = request.EventTypeName,
            ReferenceDate = request.ReferenceDate,
            CalculationTypeName = request.CalculationTypeName,
            EmployeeCodSeniorNumCad = request.EmployeeCodSeniorNumCad,
            CollaboratorTypeCodeSeniorTipCol = request.CollaboratorTypeCodeSeniorTipCol,
            CompanyCodSeniorNumEmp = request.CompanyCodSeniorNumEmp,
            PayrollPeriodCodSeniorCodCal = request.PayrollPeriodCodSeniorCodCal,
            EventTypeCodSeniorTipEve = request.EventTypeCodSeniorTipEve,
            CalculationTypeCodSeniorTipCal = request.CalculationTypeCodSeniorTipCal
        };

        await _payrollRepository.AddAsync(payroll, cancellationToken);
        await _payrollRepository.UnitOfWork.Commit();

        PayrollResponse response = payroll.Adapt<PayrollResponse>();
        return response;

    }

}
