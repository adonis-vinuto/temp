using Application.Common.Responses;
using Application.DTOs.SalaryHistory;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.People.SalaryHistory.SearchByIdKnowledge;
public class SearchSalaryHistoryIdKnowledgeHandler : BaseHandler
{
    private readonly ISalaryHistoryRepository _salaryHistoryRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;

    public SearchSalaryHistoryIdKnowledgeHandler(ISalaryHistoryRepository salaryHistoryRepository, IAuthenticationService authenticationService, IModuleService moduleService)
    {
        _salaryHistoryRepository = salaryHistoryRepository;
        _authenticationService = authenticationService;
        _moduleService = moduleService;
    }

    public async Task<ErrorOr<PagedResponse<SalaryHistoryResponse>>> Handle(Guid idKnowledge, Module module, int pagina, int tamanhoPagina, CancellationToken cancellationToken)
    {
        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        if (!_moduleService.HasAccessToModule(user, module))
        {
            return UserErrors.ModuleAccessDenied;
        }

        PagedResponse<Domain.Entities.SalaryHistory> pagedSalaries = await _salaryHistoryRepository.SearchSalaryHistoryByIdKnowledge(idKnowledge, pagina, tamanhoPagina, cancellationToken);

        List<SalaryHistoryResponse> salariesResponse = pagedSalaries!.Itens
            .Adapt<List<SalaryHistoryResponse>>();

        return PagedResponse<SalaryHistoryResponse>.Create(
            salariesResponse,
            pagedSalaries.TotalItens,
            pagedSalaries.Indice,
            pagedSalaries.TamanhoPagina);
    }
}
