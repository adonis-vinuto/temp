using ErrorOr;
using MapsterMapper;
using Application.DTOs.Session;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;

namespace Application.Handlers.Session.SearchTwilioByIdAgent;

public class SearchSessionTwilioByIdAgentHandler : BaseHandler
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public SearchSessionTwilioByIdAgentHandler(
        ISessionRepository sessionRepository,
        IAuthenticationService authenticationService,
        IMapper mapper)
    {
        _sessionRepository = sessionRepository;
        _authenticationService = authenticationService;
        _mapper = mapper;
    }

    public async Task<ErrorOr<SessionResponse>> Handle(
        SearchSessionTwilioByIdAgentRequest request,
        Module module,
        CancellationToken cancellationToken = default)
    {
        if (Validate(request, new SearchSessionTwilioByIdAgentRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        Domain.Entities.ChatSession? session = await _sessionRepository.SearchChatSessionTwilioByIdAgent(
            request.IdAgent,
            module,
            cancellationToken);

        if (session is null)
        {
            return ChatSessionErrors.ChatSessionNotFound;
        }

        return _mapper.Map<SessionResponse>(session);
    }
}