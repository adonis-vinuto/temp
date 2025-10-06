using Application.Handlers;
using ErrorOr;
using Mapster;
using Application.DTOs.Agent;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;

namespace Application.Handlers.Agent.Edit;

public class EditAgentHandler : BaseHandler
{
    private readonly IAgentRepository _agentRepository;
    private readonly IAuthenticationService _authenticationService;

    public EditAgentHandler(IAgentRepository agentRepository, IAuthenticationService authenticationService)
    {
        _agentRepository = agentRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<AgentResponse>> Handle(EditAgentRequest request, Module module, CancellationToken cancellationToken)
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
        
        Domain.Entities.Agent? agent = await _agentRepository.SearchByIdAsync(request.Id, user.Organizations.FirstOrDefault()!, module, cancellationToken);

        if (agent is null)
        {
            return AgentErrors.AgentNotFound;
        }

        agent.Name = request.Name ?? agent.Name;
        agent.Description = request.Description ?? agent.Description;
        
        _agentRepository.Edit(agent);
        await _agentRepository.UnitOfWork.Commit();
        
        AgentResponse agentResponse = agent.Adapt<AgentResponse>();
        return agentResponse;
    }
}