using Application.DTOs.Agent;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.Agent.Remove;

public sealed class RemoveAgentHandler : BaseHandler
{
    private readonly IAgentRepository _agentRepository;
    private readonly IAuthenticationService _authenticationService;

    public RemoveAgentHandler(IAgentRepository agentRepository,
        IAuthenticationService authenticationService)
    {
        _agentRepository = agentRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<AgentResponse>> Handle(RemoveAgentRequest request, Module module, CancellationToken cancellationToken)
    {
        if (Validate(request, new RemoveAgentRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }
        
        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }
        
        Domain.Entities.Agent? agent = await _agentRepository.SearchByIdAsync(request.Id, user.Organizations.FirstOrDefault()!, module, cancellationToken);

        if (agent is null)
        {
            return AgentErrors.AgentNotFound;
        }

        _agentRepository.Remove(agent);
        await _agentRepository.UnitOfWork.Commit();
        
        AgentResponse agentResponse = agent.Adapt<AgentResponse>();
        return agentResponse;
    }
}