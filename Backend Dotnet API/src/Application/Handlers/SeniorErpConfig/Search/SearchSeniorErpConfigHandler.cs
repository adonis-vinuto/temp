using Application.Common.Responses;
using Application.DTOs.Agent;
using Application.DTOs.SeniorErpConfig;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Errors;
using ErrorOr;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.SeniorErpConfig.Search;
public class SearchSeniorErpConfigHandler : BaseHandler
{
    private readonly ISeniorErpConfigRepository _seniorErpConfigRepository;
    private readonly IAuthenticationService _authenticationService;

    public SearchSeniorErpConfigHandler(ISeniorErpConfigRepository seniorErpConfigRepository,
        IAuthenticationService authenticationService)
    {
        _seniorErpConfigRepository = seniorErpConfigRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<PagedResponse<SeniorErpConfigResponse>>> Handle(int pagina, int tamanhoPagina, CancellationToken cancellationToken)
    {
        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        PagedResponse<Domain.Entities.SeniorErpConfig> pagedSeniorErpConfig = await _seniorErpConfigRepository.PagedSearchAsync(pagina, tamanhoPagina, cancellationToken);

        List<SeniorErpConfigResponse> seniorResponse = pagedSeniorErpConfig!.Itens
            .Adapt<List<SeniorErpConfigResponse>>();

        return PagedResponse<SeniorErpConfigResponse>.Create(
            seniorResponse,
            pagedSeniorErpConfig.TotalItens,
            pagedSeniorErpConfig.Indice,
            pagedSeniorErpConfig.TamanhoPagina);
    }
}
