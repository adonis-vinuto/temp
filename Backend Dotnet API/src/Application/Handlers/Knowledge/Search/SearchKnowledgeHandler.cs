using Application.Common.Responses;
using Application.DTOs.Knowledge;
using ErrorOr;
using Mapster;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using Microsoft.VisualBasic;

namespace Application.Handlers.Knowledge.Search;

public class SearchKnowledgesHandler : BaseHandler
{
    private readonly IKnowledgeRepository _knowledgeRepository;
    private readonly IAuthenticationService _authenticationService;

    public SearchKnowledgesHandler(IKnowledgeRepository knowledgeRepository, IAuthenticationService authenticationService)
    {
        _knowledgeRepository = knowledgeRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<PagedResponse<KnowledgeResponse>>> Handle(int pagina, int tamanhoPagina, Module module, Guid? idAgent, CancellationToken cancellationToken)
    {
        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        PagedResponse<Domain.Entities.Knowledge> pagedKnowledges = await _knowledgeRepository.PagedSearchAsync(module, idAgent, pagina, tamanhoPagina, cancellationToken);

        List<KnowledgeResponse> knowledgesResponse = pagedKnowledges!.Itens
            .Adapt<List<KnowledgeResponse>>();

        return PagedResponse<KnowledgeResponse>.Create(
            knowledgesResponse,
            pagedKnowledges.TotalItens,
            pagedKnowledges.Indice,
            pagedKnowledges.TamanhoPagina);
    }
}
