using Application.DTOs.TwilioConfig;
using Application.Interfaces.Repositories;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.TwilioConfig.SearchById;

public class SearchTwilioConfigByIdHandler : BaseHandler
{
    private readonly ITwilioConfigRepository _twilioConfigRepository;

    public SearchTwilioConfigByIdHandler(ITwilioConfigRepository twilioConfigRepository)
    {
        _twilioConfigRepository = twilioConfigRepository;
    }

    public async Task<ErrorOr<TwilioConfigResponse>> Handle(
        SearchTwilioConfigByIdRequest request,
        Module module,
        CancellationToken cancellationToken)
    {
        if (Validate(request, new SearchTwilioConfigByIdRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        Domain.Entities.TwilioConfig? twilioConfig = await _twilioConfigRepository.SearchByIdAsync(
            request.Id,
            module,
            cancellationToken
        );

        if (twilioConfig is null)
        {
            return TwilioConfigErrors.TwilioConfigNotFound;
        }

        TwilioConfigResponse response = twilioConfig.Adapt<TwilioConfigResponse>();

        return response;
    }
}
