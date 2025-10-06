using Application.DTOs.Employee;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.People.Employee.Edit;

public class EditEmployeeHandler : BaseHandler
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAuthenticationService _authenticationService;

    public EditEmployeeHandler(
        IEmployeeRepository employeeRepository,
        IAuthenticationService authenticationService)
    {
        _employeeRepository = employeeRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<EmployeeResponse>> Handle(
        EditEmployeeRequest request,
        Guid idKnowledge,
        CancellationToken cancellationToken)
    {
        if (Validate(request, new EditEmployeeRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();
        
        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        Domain.Entities.Employee? employee = await _employeeRepository.SearchByIdAsync(
            idKnowledge,
            request.Id,
            cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.EmployeeNotFound;
        }
        
        employee = request.Adapt(employee);
    
        _employeeRepository.Edit(employee);
        await _employeeRepository.UnitOfWork.Commit();

        EmployeeResponse response = employee.Adapt<EmployeeResponse>();
        return response;
    }
}
