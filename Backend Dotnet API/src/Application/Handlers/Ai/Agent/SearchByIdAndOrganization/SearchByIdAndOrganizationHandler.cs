using Application.Handlers;
using ErrorOr;
using MapsterMapper;
using Application.DTOs.Agent;
using Application.Interfaces.Repositories;
using Domain.Enums;
using Domain.Errors;

namespace Application.Handlers.Ai.Agent.SearchByIdAndOrganization;

public class SearchByIdAndOrganizationHandler : BaseHandler
{
    private readonly IAgentRepository _agentRepository;
    private readonly IMapper _mapper;

    public SearchByIdAndOrganizationHandler(IAgentRepository agentRepository, IMapper mapper)
    {
        _agentRepository = agentRepository;
        _mapper = mapper;
    }

    public async Task<ErrorOr<AiAgentResponse>> Handle(SearchByIdAndOrganizationRequest request, Module module, string organization, CancellationToken cancellationToken = default)
    {
        if (Validate(request, new SearchByIdAndOrganizationRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        Domain.Entities.Agent? agent = await _agentRepository.SearchByIdAsync(request.Id, organization, module, cancellationToken);

        if (agent is null)
        {
            return AgentErrors.AgentNotFound;
        }

        return _mapper.Map<AiAgentResponse>(agent);
    }
}