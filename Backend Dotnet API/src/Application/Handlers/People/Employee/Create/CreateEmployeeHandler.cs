using Application.DTOs.Employee;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.People.Employee.Create;

public class CreateEmployeeHandler : BaseHandler
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IKnowledgeRepository _knowledgeRepository;
    private readonly IAuthenticationService _authenticationService;

    public CreateEmployeeHandler(
        IEmployeeRepository employeeRepository,
        IKnowledgeRepository knowledgeRepository,
        IAuthenticationService authenticationService)
    {
        _employeeRepository = employeeRepository;
        _knowledgeRepository = knowledgeRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<EmployeeResponse>> Handle(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        if (Validate(request, new CreateEmployeeRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        Domain.Entities.Knowledge knowledge = await _knowledgeRepository.SearchByIdAsync(
            request.IdKnowledge,
            Module.People,
            cancellationToken);

        if (knowledge is null)
        {
            return KnowledgeErrors.KnowledgeWithoutPermission;
        }

        var employee = new Domain.Entities.Employee
        {
            IdKnowledge = request.IdKnowledge,
            CompanyName = request.CompanyName,
            FullName = request.FullName,
            AdmissionDate = request.AdmissionDate,
            TerminationDate = request.TerminationDate,
            StatusDescription = request.StatusDescription,
            BirthDate = request.BirthDate,
            CostCneterName = request.CostCneterName,
            Salary = request.Salary,
            ComplementarySalary = request.ComplementarySalary,
            SalaryEffectiveDate = request.SalaryEffectiveDate,
            Gender = request.Gender,
            StreetAddress = request.StreetAddress,
            AddressNumber = request.AddressNumber,
            CityName = request.CityName,
            Race = request.Race,
            PostalCode = request.PostalCode,
            CompanyCodSeniorNumEmp = request.CompanyCodSeniorNumemp,
            EmployeeCodSeniorNumCad = request.EmployeeCodSeniorNumcad,
            CollaboratorTypeCodeSeniorTipeCol = request.CollaboratorTypeCodeSeniorTipcol,
            StatusCodSenior = request.StatusCodSeniorSitafa,
            CostCenterCodSeniorCodCcu = request.CostCenterCodSeniorCodccu,
        };

        await _employeeRepository.AddAsync(employee, cancellationToken);
        await _employeeRepository.UnitOfWork.Commit();

        EmployeeResponse response = employee.Adapt<EmployeeResponse>();
        return response;
    }
}
