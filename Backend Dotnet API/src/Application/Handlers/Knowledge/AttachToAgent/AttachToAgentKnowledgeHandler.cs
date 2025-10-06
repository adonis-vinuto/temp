using Application.DTOs.Knowledge;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.Knowledge.AttachToAgent;

public class AttachToAgentKnowledgeHandler : BaseHandler
{
    private readonly IKnowledgeRepository _knowledgeRepository;
    private readonly IAgentRepository _agentRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;

    public AttachToAgentKnowledgeHandler(IKnowledgeRepository knowledgeRepository,
        IAuthenticationService authenticationService,
        IModuleService moduleService,
        IAgentRepository agentRepository)
    {
        _knowledgeRepository = knowledgeRepository;
        _authenticationService = authenticationService;
        _moduleService = moduleService;
        _agentRepository = agentRepository;
    }

    public async Task<ErrorOr<KnowledgeResponse>> Handle(AttachToAgentKnowledgeRequest request, Module module, CancellationToken cancellationToken)
    {
        if (Validate(request, new AttachToAgentKnowledgeRequestValidator()) is var resultado && resultado.Any())
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

        Domain.Entities.Knowledge? knowledge = await _knowledgeRepository.SearchByIdAsync(request.Id, module, cancellationToken);

        if (knowledge is null)
        {
            return KnowledgeErrors.KnowledgeNotFound;
        }

        Domain.Entities.Agent agent = await _agentRepository.SearchByIdAsync(request.IdAgent, user.Organizations.FirstOrDefault()!, module, cancellationToken);

        if (agent is null)
        {
            return AgentErrors.AgentNotFound;
        }

        if (knowledge.HasAgent(request.IdAgent))
        {
            knowledge.RemoveAgent(agent);
        }
        else
        {
            knowledge.AddAgent(agent);
        }

        _knowledgeRepository.Update(knowledge);
        await _knowledgeRepository.UnitOfWork.Commit();

        KnowledgeResponse knowledgeResponse = knowledge.Adapt<KnowledgeResponse>();
        return knowledgeResponse;
    }
}