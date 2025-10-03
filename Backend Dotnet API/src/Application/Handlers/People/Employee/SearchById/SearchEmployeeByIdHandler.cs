using ErrorOr;
using MapsterMapper;
using Application.DTOs.Employee;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;

namespace Application.Handlers.People.Employee.SearchById;

public class SearchEmployeeByIdHandler : BaseHandler
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public SearchEmployeeByIdHandler(
        IEmployeeRepository employeeRepository,
        IAuthenticationService authenticationService,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _authenticationService = authenticationService;
        _mapper = mapper;
    }

    public async Task<ErrorOr<EmployeeResponse>> Handle(
        SearchEmployeeByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        if (Validate(request, new SearchEmployeeByIdRequestValidator()) is var resultado && resultado.Any())
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

        return _mapper.Map<EmployeeResponse>(employee);
    }
}