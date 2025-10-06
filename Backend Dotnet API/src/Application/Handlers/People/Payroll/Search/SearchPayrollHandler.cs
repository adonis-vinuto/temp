using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Responses;
using Application.DTOs.Knowledge.People.Payroll;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Entities;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.People.Payroll.Search;

public class SearchPayrollHandler : BaseHandler
{
    private readonly IPayrollRepository _payrollRepository;
    private readonly IAuthenticationService _authenticationService;

    public SearchPayrollHandler(
        IPayrollRepository payrollRepository,
        IAuthenticationService authenticationService)
    {
        _payrollRepository = payrollRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<PagedResponse<PayrollResponse>>> Handle(Guid idKnowledge, int pagina,
    int tamanhoPagina, CancellationToken cancellationToken)
    {
        UserAuthInfoResponse? user = _authenticationService.GetUserAuthInfo();
        
        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        PagedResponse<Domain.Entities.Payroll> pagedPayroll = await _payrollRepository
            .PagedSearchAsync(idKnowledge, pagina, tamanhoPagina, cancellationToken);

        List<PayrollResponse> payrollResponse = pagedPayroll.Itens.Adapt<List<PayrollResponse>>();

        return PagedResponse<PayrollResponse>.Create(
            payrollResponse,
            pagedPayroll.TotalItens,
            pagedPayroll.Indice,
            pagedPayroll.TamanhoPagina);
    }

}
