using Application.DTOs.SeniorErpConfig;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.SeniorErpConfig.AttachToAgent;

public class AttachToAgentSeniorErpConfigHandler : BaseHandler
{
    private readonly ISeniorErpConfigRepository _seniorErpConfigRepository;
    private readonly IAgentRepository _agentRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;

    public AttachToAgentSeniorErpConfigHandler(ISeniorErpConfigRepository seniorErpConfigRepository,
        IAuthenticationService authenticationService,
        IModuleService moduleService,
        IAgentRepository agentRepository)
    {
        _seniorErpConfigRepository = seniorErpConfigRepository;
        _authenticationService = authenticationService;
        _moduleService = moduleService;
        _agentRepository = agentRepository;
    }

    public async Task<ErrorOr<SeniorErpConfigResponse>> Handle(AttachToAgentSeniorErpConfigRequest request, Module module, CancellationToken cancellationToken)
    {
        if (Validate(request, new AttachToAgentSeniorErpConfigRequestValidator()) is var resultado && resultado.Any())
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

        Domain.Entities.SeniorErpConfig? seniorErpConfig = await _seniorErpConfigRepository.SearchByIdAsync(request.Id, cancellationToken);

        if (seniorErpConfig is null)
        {
            return SeniorErpConfigErrors.NotFound;
        }

        Domain.Entities.Agent agent = await _agentRepository.SearchByIdAsync(request.IdAgent, user.Organizations.FirstOrDefault()!, module, cancellationToken);

        if (agent is null)
        {
            return AgentErrors.AgentNotFound;
        }

        if (seniorErpConfig.HasAgent(request.IdAgent))
        {
            seniorErpConfig.RemoveAgent(agent);
        }
        else
        {
            seniorErpConfig.AddAgent(agent);
        }

        _seniorErpConfigRepository.Update(seniorErpConfig);
        await _seniorErpConfigRepository.UnitOfWork.Commit();

        SeniorErpConfigResponse seniorErpConfigResponse = seniorErpConfig.Adapt<SeniorErpConfigResponse>();
        return seniorErpConfigResponse;
    }
}