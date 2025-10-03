using Application.DTOs.TwilioConfig;
using Application.Interfaces.Repositories;
using Domain.Errors;
using Domain.Enums;
using ErrorOr;
using Mapster;

namespace Application.Handlers.TwilioConfig.Remove;

public class RemoveTwilioConfigHandler : BaseHandler
{
    private readonly ITwilioConfigRepository _twilioConfigRepository;

    public RemoveTwilioConfigHandler(ITwilioConfigRepository twilioConfigRepository)
    {
        _twilioConfigRepository = twilioConfigRepository;
    }

    public async Task<ErrorOr<TwilioConfigResponse>> Handle(
        RemoveTwilioConfigRequest request,
        Module module,
        CancellationToken cancellationToken)
    {
        if (Validate(request, new RemoveTwilioConfigRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        Domain.Entities.TwilioConfig? twilioConfig = await _twilioConfigRepository.SearchByIdAsync(
            request.Id,
            module,
            cancellationToken);

        if (twilioConfig is null)
        {
            return TwilioConfigErrors.TwilioConfigNotFound;
        }


        _twilioConfigRepository.Remove(twilioConfig);
        await _twilioConfigRepository.UnitOfWork.Commit();

        TwilioConfigResponse response = twilioConfig.Adapt<TwilioConfigResponse>();
        return response;
    }
}
