using Application.Handlers;
using ErrorOr;
using MapsterMapper;
using Application.DTOs.Agent;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;

namespace Application.Handlers.Agent.SearchById;

public class SearchAgentByIdHandler : BaseHandler
{
    private readonly IAgentRepository _agentRepository;
    private readonly ITwilioConfigRepository _twilioConfigRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public SearchAgentByIdHandler(
        IAgentRepository agentRepository, 
        ITwilioConfigRepository twilioConfigRepository,
        IMapper mapper, 
        IAuthenticationService authenticationService)
    {
        _agentRepository = agentRepository;
        _twilioConfigRepository = twilioConfigRepository;
        _mapper = mapper;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<AgentByIdResponse>> Handle(SearchAgentByIdRequest request, Module module, CancellationToken cancellationToken = default)
    {
        if (Validate(request, new SearchAgentByIdRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        bool isOwner = user.Roles != null && user.Roles.Contains("owner");

        if (isOwner)
        {
            Domain.Entities.Agent? agent = await _agentRepository.SearchByIdAsync(request.Id, user.Organizations.FirstOrDefault()!, module, cancellationToken);

            if (agent is null)
            {
                return AgentErrors.AgentNotFound;
            }

            AgentByIdResponse response = _mapper.Map<AgentByIdResponse>(agent);

            Domain.Entities.TwilioConfig? twilioConfig = await _twilioConfigRepository.SearchByAgentIdAsync(request.Id, module, cancellationToken);

            if (twilioConfig != null)
            {
                response.TwilioConfig = _mapper.Map<AgentTwilioConfig>(twilioConfig);
            }

            return response;
        }

        else
        {
            Domain.Entities.Agent? agent = await _agentRepository.SearchByIdAndIdUserAsync(request.Id, Guid.Parse(user.IdUser), user.Organizations.FirstOrDefault()!, module, cancellationToken);

            if (agent is null)
            {
                return AgentErrors.AgentNotFound;
            }

            AgentByIdResponse response = _mapper.Map<AgentByIdResponse>(agent);

            Domain.Entities.TwilioConfig? twilioConfig = await _twilioConfigRepository.SearchByAgentIdAsync(request.Id, module, cancellationToken);

            if (twilioConfig != null)
            {
                response.TwilioConfig = _mapper.Map<AgentTwilioConfig>(twilioConfig);
            }

            return response;
        }
        
    }
}