using Application.DTOs.SalaryHistory;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using MapsterMapper;

namespace Application.Handlers.People.SalaryHistory.SearchByIdAndIdKnowledge;
public class SearchSalaryHistoryIdAndIdKnowledgeHandler : BaseHandler
{
    private readonly ISalaryHistoryRepository _salaryHistoryRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;
    private readonly IMapper _mapper;

    public SearchSalaryHistoryIdAndIdKnowledgeHandler(ISalaryHistoryRepository salaryHistoryRepository, IAuthenticationService authenticationService, IMapper mapper, IModuleService moduleService)
    {
        _salaryHistoryRepository = salaryHistoryRepository;
        _authenticationService = authenticationService;
        _mapper = mapper;
        _moduleService = moduleService;
    }

    public async Task<ErrorOr<SalaryHistoryResponse>> Handle(Guid id, Guid idKnowledge, Module module, CancellationToken cancellationToken)
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

        Domain.Entities.SalaryHistory? salary = await _salaryHistoryRepository.SearchSalaryHistoryByIdAndIdKnowledge(id, idKnowledge, cancellationToken);

        if (salary is null)
        {
            return SalaryHistoryErrors.SalaryHistoryNotFound;
        }

        return _mapper.Map<SalaryHistoryResponse>(salary);
    }
}