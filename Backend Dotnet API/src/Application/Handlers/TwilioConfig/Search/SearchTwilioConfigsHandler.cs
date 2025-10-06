using Application.Common.Responses;
using Application.DTOs.TwilioConfig;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.TwilioConfig.Search;

public class SearchTwilioConfigsHandler : BaseHandler
{
    private readonly ITwilioConfigRepository _twilioConfigRepository;
    private readonly IAuthenticationService _authenticationService;

    public SearchTwilioConfigsHandler(ITwilioConfigRepository twilioConfigRepository, IAuthenticationService authenticationService)
    {
        _twilioConfigRepository = twilioConfigRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<PagedResponse<TwilioConfigResponse>>> Handle(
        int pagina,
        int tamanhoPagina,
        Module module,
        CancellationToken cancellationToken)
    {
        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        PagedResponse<Domain.Entities.TwilioConfig> pagedResult = await _twilioConfigRepository.PagedSearchAsync(
            module,
            pagina,
            tamanhoPagina,
            cancellationToken);

        List<TwilioConfigResponse> twilioConfigResponses = pagedResult.Itens
            .Adapt<List<TwilioConfigResponse>>();

        var result = PagedResponse<TwilioConfigResponse>.Create(
            twilioConfigResponses,
            pagedResult.TotalItens,
            pagedResult.Indice,
            pagedResult.TamanhoPagina);

        return result;
    }
}
