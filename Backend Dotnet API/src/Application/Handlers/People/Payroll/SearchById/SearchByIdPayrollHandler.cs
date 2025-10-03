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
using MapsterMapper;

namespace Application.Handlers.People.Payroll.SearchById;

public class SearchByIdPayrollHandler : BaseHandler
{
    private readonly IPayrollRepository _payrollRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

     public SearchByIdPayrollHandler(IPayrollRepository payrollRepository, IMapper mapper, IAuthenticationService authenticationService)
    {
        _payrollRepository = payrollRepository;
        _mapper = mapper;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<PayrollResponse>> Handle(SearchByIdPayrollRequest request, CancellationToken cancellationToken)
    {
        if (Validate(request, new SearchByIdPayrollRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }
        
        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }
        
        Domain.Entities.Payroll? payroll = await _payrollRepository.SearchByIdAsync(request.Id, request.IdKnowledge, cancellationToken);

        if (payroll is null)
        {
            return PayrollErrors.PayrollNotFound;
        }

        return _mapper.Map<PayrollResponse>(payroll);
    }
    
}
