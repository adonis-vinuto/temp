using Application.Common.Responses;
using Application.DTOs.Agent;
using ErrorOr;
using Mapster;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using Microsoft.VisualBasic;

namespace Application.Handlers.Agent.Search;

public class SearchAgentsHandler : BaseHandler
{
    private readonly IAgentRepository _agentRepository;
    private readonly IAuthenticationService _authenticationService;

    public SearchAgentsHandler(IAgentRepository agentRepository, IAuthenticationService authenticationService)
    {
        _agentRepository = agentRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<PagedResponse<AgentResponse>>> Handle(int pagina, int tamanhoPagina, Module module, CancellationToken cancellationToken)
    {
        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }
        
        bool isOwner = user.Roles != null && user.Roles.Contains("owner");

        if (isOwner)
        {
            PagedResponse<Domain.Entities.Agent> pagedAgents = await _agentRepository.PagedSearchAsync(user.Organizations.FirstOrDefault()!, module, pagina, tamanhoPagina, cancellationToken);

            List<AgentResponse> agentsResponse = pagedAgents!.Itens
                .Adapt<List<AgentResponse>>();

            return PagedResponse<AgentResponse>.Create(
                agentsResponse,
                pagedAgents.TotalItens,
                pagedAgents.Indice,
                pagedAgents.TamanhoPagina);
        }

        else
        {
            PagedResponse<Domain.Entities.Agent> pagedAgents = await _agentRepository.PagedSearchAsyncByIdUser(Guid.Parse(user.IdUser), user.Organizations.FirstOrDefault()!, module, pagina, tamanhoPagina, cancellationToken);

            List<AgentResponse> agentsResponse = pagedAgents!.Itens
                .Adapt<List<AgentResponse>>();

            return PagedResponse<AgentResponse>.Create(
                agentsResponse,
                pagedAgents.TotalItens,
                pagedAgents.Indice,
                pagedAgents.TamanhoPagina);
        }
        
    }
}
