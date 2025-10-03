using Application.Common.Responses;
using Application.DTOs.Agent;
using Application.DTOs.DataConfig;
using ErrorOr;
using Mapster;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;

namespace Application.Handlers.DataConfig.Search;

public class SearchDataConfigsHandler : BaseHandler
{
    private readonly IDataConfigRepository _dataConfigRepository;
    private readonly IAuthenticationService _authenticationService;

    public SearchDataConfigsHandler(IDataConfigRepository dataConfigRepository,
        IAuthenticationService authenticationService)
    {
        _dataConfigRepository = dataConfigRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<PagedResponse<DataConfigResponse>>> Handle(int pagina, int tamanhoPagina, CancellationToken cancellationToken)
    {
        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }
        
        PagedResponse<Domain.Entities.DataConfig> pagedDataConfigs = await _dataConfigRepository.PagedSearchAsync(user.Organizations.FirstOrDefault()!, Module.People, pagina, tamanhoPagina, cancellationToken);

        List<DataConfigResponse> dataConfigsResponse = pagedDataConfigs!.Itens
            .Adapt<List<DataConfigResponse>>();

        return PagedResponse<DataConfigResponse>.Create(
            dataConfigsResponse,
            pagedDataConfigs.TotalItens,
            pagedDataConfigs.Indice,
            pagedDataConfigs.TamanhoPagina);
    }
}
