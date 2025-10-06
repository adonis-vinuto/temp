using Application.DTOs.SalaryHistory;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.People.SalaryHistory.Remove;

public sealed class RemoveSalaryHistoryHandler : BaseHandler
{
    private readonly ISalaryHistoryRepository _salaryHistoryRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;

    public RemoveSalaryHistoryHandler(ISalaryHistoryRepository salaryHistoryRepository,
        IAuthenticationService authenticationService,
        IModuleService moduleService)
    {
        _salaryHistoryRepository = salaryHistoryRepository;
        _authenticationService = authenticationService;
        _moduleService = moduleService;
    }

    public async Task<ErrorOr<SalaryHistoryResponse>> Handle(RemoveSalaryHistoryRequest request, Module module, CancellationToken cancellationToken)
    {
        if (Validate(request, new RemoveSalaryHistoryRequestValidator()) is var resultado && resultado.Any())
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

        Domain.Entities.SalaryHistory? salaryHistory = await _salaryHistoryRepository.SearchSalaryHistoryByIdAndIdKnowledge(request.IdSalaryHistory, request.IdKnowledge, cancellationToken);

        if (salaryHistory is null)
        {
            return SalaryHistoryErrors.SalaryHistoryNotFound;
        }

        _salaryHistoryRepository.RemoveSalaryHistoryByIdAndIdKnowledge(salaryHistory, cancellationToken);
        await _salaryHistoryRepository.UnitOfWork.Commit();

        SalaryHistoryResponse salaryHistoryResponse = salaryHistory.Adapt<SalaryHistoryResponse>();
        return salaryHistoryResponse;
    }
}