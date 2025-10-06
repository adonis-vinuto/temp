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

namespace Application.Handlers.People.Payroll.Remove;

public sealed class RemovePayrollHandler : BaseHandler
{

    private readonly IPayrollRepository _payrollRepository;
    private readonly IAuthenticationService _authenticationService;

    public RemovePayrollHandler(
        IPayrollRepository payrollRepository,
        IAuthenticationService authenticationService)
    {
        _payrollRepository = payrollRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<PayrollResponse>> Handle(RemovePayrollRequest request, CancellationToken cancellationToken)
    {
        if (Validate(request, new RemovePayrollRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }
        
        UserAuthInfoResponse? user = _authenticationService.GetUserAuthInfo();
        
        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        Domain.Entities.Payroll? payroll = await _payrollRepository.SearchByIdAsync(request.IdPayroll, request.IdKnowledge, cancellationToken);

        if (payroll is null)
        {
            return PayrollErrors.PayrollNotFound;
        }

        _payrollRepository.Remove(payroll);
        await _payrollRepository.UnitOfWork.Commit();

        return payroll.Adapt<PayrollResponse>();
    }
}
