using Application.DTOs.TwilioConfig;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.TwilioConfig.Create;

public class CreateTwilioConfigHandler : BaseHandler
{
    private readonly ITwilioConfigRepository _twilioConfigRepository;
    private readonly IAgentRepository _agentRepository;
    private readonly IAuthenticationService _authenticationService;

    public CreateTwilioConfigHandler(
        ITwilioConfigRepository twilioConfigRepository,
        IAgentRepository agentRepository,
        IAuthenticationService authenticationService)
    {
        _twilioConfigRepository = twilioConfigRepository;
        _agentRepository = agentRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<TwilioConfigResponse>> Handle(
        CreateTwilioConfigRequest request,
        Module module,
        CancellationToken cancellationToken)
    {
        if (Validate(request, new CreateTwilioConfigRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        Domain.Entities.Agent? agent = await _agentRepository.SearchByIdAsync(
            request.IdAgent, 
            user.Organizations.FirstOrDefault()!,
            module,
            cancellationToken);
       
        if (agent is null)
        {
            return AgentErrors.AgentNotFound;
        }

        Domain.Entities.TwilioConfig? existingConfig = await _twilioConfigRepository.SearchByAgentIdAsync(
            request.IdAgent,
            module,
            cancellationToken);

        if (existingConfig is not null)
        {
            return TwilioConfigErrors.TwilioConfigAlreadyExists;
        }

        var twilioConfig = new Domain.Entities.TwilioConfig
        {
            IdAgent = request.IdAgent,
            AccountSid = request.AccountSid,
            AuthToken = request.AuthToken,
            WebhookUrl = request.WebhookUrl
        };

        await _twilioConfigRepository.AddAsync(twilioConfig, cancellationToken);
        await _twilioConfigRepository.UnitOfWork.Commit();

        TwilioConfigResponse response = twilioConfig.Adapt<TwilioConfigResponse>();
        
        return response;
    }
}
