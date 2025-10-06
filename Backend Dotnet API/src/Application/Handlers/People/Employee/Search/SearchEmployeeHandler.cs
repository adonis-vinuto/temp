using Application.Common.Responses;
using Application.DTOs.Employee;
using ErrorOr;
using Mapster;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;

namespace Application.Handlers.People.Employee.Search;

public class SearchEmployeeHandler : BaseHandler
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAuthenticationService _authenticationService;

    public SearchEmployeeHandler(
        IEmployeeRepository employeeRepository,
        IAuthenticationService authenticationService)
    {
        _employeeRepository = employeeRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<PagedResponse<EmployeeResponse>>> Handle(
        Guid idKnowledge,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken)
    {
        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        PagedResponse<Domain.Entities.Employee> pagedEmployees = await _employeeRepository.PagedSearchAsync(
            idKnowledge,
            pagina,
            tamanhoPagina,
            cancellationToken);

        List<EmployeeResponse> employeesResponse = pagedEmployees!.Itens
            .Adapt<List<EmployeeResponse>>();

        return PagedResponse<EmployeeResponse>.Create(
            employeesResponse,
            pagedEmployees.TotalItens,
            pagedEmployees.Indice,
            pagedEmployees.TamanhoPagina);
    }
}
