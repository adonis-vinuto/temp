using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Responses;
using Application.DTOs.SeniorHcmConfig;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.SeniorHcmConfig.Search;

public class SearchSeniorHcmConfigHandler : BaseHandler
{
    private readonly ISeniorHcmConfigRepository _seniorHcmConfigRepository;
    private readonly IAuthenticationService _authenticationService;

    public SearchSeniorHcmConfigHandler(ISeniorHcmConfigRepository seniorHcmConfigRepository,
        IAuthenticationService authenticationService)
    {
        _seniorHcmConfigRepository = seniorHcmConfigRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<PagedResponse<SeniorHcmConfigResponse>>> Handle(int pagina, int tamanhoPagina, CancellationToken cancellationToken)
    {
        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return SeniorErpConfigErrors.NotFound;
        }

        PagedResponse<Domain.Entities.SeniorHcmConfig> pagedSeniorHcmConfig = await _seniorHcmConfigRepository
        .PagedSearchAsync(pagina, tamanhoPagina, cancellationToken);

        List<SeniorHcmConfigResponse> seniorResponse = pagedSeniorHcmConfig!.Itens
            .Adapt<List<SeniorHcmConfigResponse>>();

        return PagedResponse<SeniorHcmConfigResponse>.Create(
            seniorResponse,
            pagedSeniorHcmConfig.TotalItens,
            pagedSeniorHcmConfig.Indice,
            pagedSeniorHcmConfig.TamanhoPagina);
    }
}
