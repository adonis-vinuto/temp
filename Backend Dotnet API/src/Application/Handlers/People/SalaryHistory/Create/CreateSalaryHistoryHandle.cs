using Application.DTOs.SalaryHistory;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.People.SalaryHistory.Create;
public class CreateSalaryHistoryHandler : BaseHandler
{
    private readonly ISalaryHistoryRepository _salaryHistoryRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IModuleService _moduleService;

    public CreateSalaryHistoryHandler(ISalaryHistoryRepository salaryHistoryRepository, IAuthenticationService authenticationService, IModuleService moduleService, IEmployeeRepository employeeRepository)
    {
        _salaryHistoryRepository = salaryHistoryRepository;
        _authenticationService = authenticationService;
        _moduleService = moduleService;
        _employeeRepository = employeeRepository;
    }

    public async Task<ErrorOr<SalaryHistoryResponse>> Handle(Module module, CreateSalaryHistoryRequest request, CancellationToken cancellationToken)
    {
        if (Validate(request, new CreateSalaryHistoryRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        if (!_moduleService.HasAccessToModule(user, module))
        {
            return UserErrors.ModuleAccessDenied;
        }

        Domain.Entities.Employee employee = await _employeeRepository.SearchByIdEmployeeAsync(request.IdEmployee, cancellationToken);
        
        if (employee is null)
        {
            return EmployeeErrors.EmployeeNotFound;
        }

        var salaryHistory = new Domain.Entities.SalaryHistory
        {
            ChangeDate = DateTime.Now,
            CompanyCodSeniorNumEmp = request.CompanyCodSeniorNumEmp,
            IdEmployee = request.IdEmployee,
            NewSalary = request.NewSalary,
            MotiveCodSeniorCodMot = request.MotiveCodSeniorCodMot,
            EmployeeCodSeniorNumCad = request.EmployeeCodSeniorNumCad
        };

        await _salaryHistoryRepository.CreateSalaryHistory(salaryHistory, cancellationToken);
        await _salaryHistoryRepository.UnitOfWork.Commit();

        return salaryHistory.Adapt<SalaryHistoryResponse>();
    }
}
