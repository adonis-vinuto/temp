using Application.DTOs.SalaryHistory;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.People.SalaryHistory.Edit;

public class EditSalaryHistoryHandler : BaseHandler
{
    private readonly ISalaryHistoryRepository _salaryHistoryRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;
    private readonly IEmployeeRepository _employeeRepository;

    public EditSalaryHistoryHandler(ISalaryHistoryRepository salaryHistoryRepository, IAuthenticationService authenticationService, IModuleService moduleService, IEmployeeRepository employeeRepository)
    {
        _salaryHistoryRepository = salaryHistoryRepository;
        _authenticationService = authenticationService;
        _moduleService = moduleService;
        _employeeRepository = employeeRepository;
    }

    public async Task<ErrorOr<SalaryHistoryResponse>> Handle(Guid idSalary, Guid idKnowlege, Module module, EditSalaryHistoryRequest request, CancellationToken cancellationToken)
    {
        if (Validate(request, new EditAgentRequestValidator()) is var resultado && resultado.Any())
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

        if (request.IdEmployee != null)
        {
            Domain.Entities.Employee? employee = await _employeeRepository.SearchByIdEmployeeAsync(request.IdEmployee, cancellationToken);

            if (employee is null)
            {
                return EmployeeErrors.EmployeeNotFound;
            }
        }

        Domain.Entities.SalaryHistory? salary = await _salaryHistoryRepository.SearchSalaryHistoryByIdAndIdKnowledge(idSalary, idKnowlege, cancellationToken);

        if (salary is null)
        {
            return SalaryHistoryErrors.SalaryHistoryNotFound;
        }

        salary.NewSalary = request.NewSalary ?? salary.NewSalary;
        salary.ChangeDate = DateTime.Now;
        salary.EmployeeCodSeniorNumCad = request.EmployeeCodSeniorNumCad ?? salary.EmployeeCodSeniorNumCad;
        salary.CompanyCodSeniorNumEmp = request.CompanyCodSeniorNumEmp ?? salary.CompanyCodSeniorNumEmp;
        salary.MotiveCodSeniorCodMot = request.MotiveCodSeniorCodMot ?? salary.MotiveCodSeniorCodMot;
        salary.IdEmployee = request.IdEmployee ?? salary.IdEmployee;

        _salaryHistoryRepository.EditSalaryHistory(salary);
        await _salaryHistoryRepository.UnitOfWork.Commit();

        SalaryHistoryResponse salaryResponse = salary.Adapt<SalaryHistoryResponse>();
        return salaryResponse;
    }
}