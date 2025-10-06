using Application.DTOs.Employee;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.People.Employee.Remove;

public sealed class RemoveEmployeeHandler : BaseHandler
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAuthenticationService _authenticationService;

    public RemoveEmployeeHandler(
        IEmployeeRepository employeeRepository,
        IAuthenticationService authenticationService)
    {
        _employeeRepository = employeeRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<EmployeeResponse>> Handle(RemoveEmployeeRequest request, CancellationToken cancellationToken)
    {
        if (Validate(request, new RemoveEmployeeRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }
        
        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        Domain.Entities.Employee? employee = await _employeeRepository.SearchByIdAsync(
            request.IdKnowledge,
            request.IdEmployee,
            cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.EmployeeNotFound;
        }

        _employeeRepository.Remove(employee);
        await _employeeRepository.UnitOfWork.Commit();

        EmployeeResponse employeeResponse = employee.Adapt<EmployeeResponse>();
        return employeeResponse;
    }
}