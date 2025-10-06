using Application.DTOs.SeniorHcmConfig;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.SeniorHcmConfig.AttachToAgent;

public class AttachToAgentSeniorHcmConfigHandler : BaseHandler
{
    private readonly ISeniorHcmConfigRepository _seniorHcmConfigRepository;
    private readonly IAgentRepository _agentRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;

    public AttachToAgentSeniorHcmConfigHandler(ISeniorHcmConfigRepository seniorHcmConfigRepository,
        IAuthenticationService authenticationService,
        IModuleService moduleService,
        IAgentRepository agentRepository)
    {
        _seniorHcmConfigRepository = seniorHcmConfigRepository;
        _authenticationService = authenticationService;
        _moduleService = moduleService;
        _agentRepository = agentRepository;
    }

    public async Task<ErrorOr<SeniorHcmConfigResponse>> Handle(AttachToAgentSeniorHcmConfigRequest request, Module module, CancellationToken cancellationToken)
    {
        if (Validate(request, new AttachToAgentSeniorHcmConfigRequestValidator()) is var resultado && resultado.Any())
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

        Domain.Entities.SeniorHcmConfig? seniorHcmConfig = await _seniorHcmConfigRepository.SearchByIdAsync(request.Id, cancellationToken);

        if (seniorHcmConfig is null)
        {
            return SeniorHcmConfigErrors.NotFound;
        }

        Domain.Entities.Agent agent = await _agentRepository.SearchByIdAsync(request.IdAgent, user.Organizations.FirstOrDefault()!, module, cancellationToken);

        if (agent is null)
        {
            return AgentErrors.AgentNotFound;
        }

        if (seniorHcmConfig.HasAgent(request.IdAgent))
        {
            seniorHcmConfig.RemoveAgent(agent);
        }
        else
        {
            seniorHcmConfig.AddAgent(agent);
        }

        _seniorHcmConfigRepository.Update(seniorHcmConfig);
        await _seniorHcmConfigRepository.UnitOfWork.Commit();

        SeniorHcmConfigResponse seniorHcmConfigResponse = seniorHcmConfig.Adapt<SeniorHcmConfigResponse>();
        return seniorHcmConfigResponse;
    }
}