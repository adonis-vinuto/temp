using Application.DTOs.Agent;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.Agent.Create;

public class CreateAgentHandler : BaseHandler
{
    private readonly IAgentRepository _agentRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;

    public CreateAgentHandler(IAgentRepository agentRepository, 
        IAuthenticationService authenticationService,
        IModuleService moduleService)
    {
        _agentRepository = agentRepository;
        _authenticationService = authenticationService;
        _moduleService = moduleService;
    }

    public async Task<ErrorOr<AgentResponse>> Handle(CreateAgentRequest request, Module module, CancellationToken cancellationToken)
    {
        if (Validate(request, new CreateAgentRequestValidator()) is var resultado && resultado.Any())
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

        var agent = new Domain.Entities.Agent
        {
            Organization = user.Organizations.FirstOrDefault()!,
            Module = module,
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            IdUser = Guid.Parse(user.IdUser)
        };

        await _agentRepository.AddAsync(agent, cancellationToken);
        await _agentRepository.UnitOfWork.Commit();
        
        AgentResponse agentResponse = agent.Adapt<AgentResponse>();
        return agentResponse;
    }
}