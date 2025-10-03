using Application.DTOs.TwilioConfig;
using Application.Interfaces.Repositories;
using Domain.Errors;
using Domain.Enums;
using ErrorOr;
using Mapster;

namespace Application.Handlers.TwilioConfig.Edit;

public class EditTwilioConfigHandler : BaseHandler
{
    private readonly ITwilioConfigRepository _twilioConfigRepository;

    public EditTwilioConfigHandler(ITwilioConfigRepository twilioConfigRepository)
    {
        _twilioConfigRepository = twilioConfigRepository;
    }

    public async Task<ErrorOr<TwilioConfigResponse>> Handle(
        EditTwilioConfigRequest request,
        Module module,
        CancellationToken cancellationToken)
    {
        if (Validate(request, new EditTwilioConfigRequestValidator()) is var resultado && resultado.Any())
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

        twilioConfig.AccountSid = request.AccountSid ?? twilioConfig.AccountSid;
        twilioConfig.AuthToken = request.AuthToken ?? twilioConfig.AuthToken;
        twilioConfig.WebhookUrl = request.WebhookUrl ?? twilioConfig.WebhookUrl;

        _twilioConfigRepository.Edit(twilioConfig);
        await _twilioConfigRepository.UnitOfWork.Commit();
        
        TwilioConfigResponse response = twilioConfig.Adapt<TwilioConfigResponse>();

        return response;
    }
}
