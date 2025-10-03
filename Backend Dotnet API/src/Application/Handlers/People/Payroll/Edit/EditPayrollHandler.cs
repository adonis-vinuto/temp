using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Knowledge.People.Payroll;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.People.Payroll.Edit;

public class EditPayrollHandler : BaseHandler
{
    private readonly IPayrollRepository _payrollRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IEmployeeRepository _employeeRepository;

    public EditPayrollHandler(
        IPayrollRepository payrollRepository,
        IAuthenticationService authenticationService,
        IEmployeeRepository employeeRepository)
    {
        _payrollRepository = payrollRepository;
        _authenticationService = authenticationService;
        _employeeRepository = employeeRepository;
    }

    public async Task<ErrorOr<PayrollResponse>> Handle(EditPayrollRequest request, Guid idKnowledge, CancellationToken cancellationToken)
    {
        if (Validate(request, new EditPayrollRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        if (request.IdEmployee != null)
        {
            Domain.Entities.Employee? employee = await _employeeRepository.SearchByIdAsync(
                idKnowledge, request.IdEmployee, cancellationToken);


            if (employee is null)
            {
                return EmployeeErrors.EmployeeNotFound;
            }
        }

        Domain.Entities.Payroll? payroll = await _payrollRepository.SearchByIdAsync(request.Id, idKnowledge, cancellationToken);

        if (payroll is null)
        {
            return PayrollErrors.PayrollNotFound;
        }

        payroll = request.Adapt(payroll);

        _payrollRepository.Edit(payroll);
        await _payrollRepository.UnitOfWork.Commit();

        PayrollResponse response = payroll.Adapt<PayrollResponse>();

        return response;
    }
}
