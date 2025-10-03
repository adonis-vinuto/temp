using ErrorOr;
using Application.DTOs.Session;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using System.Globalization;
using System.Linq;

namespace Application.Handlers.Session.SearchByIdAgent;

public class SearchSessionByIdAgentHandler : BaseHandler
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IAuthenticationService _authenticationService;

    public SearchSessionByIdAgentHandler(
        ISessionRepository sessionRepository,
        IAuthenticationService authenticationService)
    {
        _sessionRepository = sessionRepository;
        _authenticationService = authenticationService;
    }

    public async Task<ErrorOr<List<SessionResponse>>> Handle(
        SearchSessionByIdAgentRequest request,
        Module module,
        CancellationToken cancellationToken = default)
    {
        if (Validate(request, new SearchSessionByIdAgentRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        if (user.IdUser != request.IdUser.ToString())
        {
            return UserErrors.UserNotFound;
        }

        List<Domain.Entities.ChatSession> sessions = await _sessionRepository.SearchChatSessionByIdAgent(
            request.IdAgent,
            user.IdUser,
            module,
            cancellationToken);

        if (sessions is null || !sessions.Any())
        {
            return ChatSessionErrors.ChatSessionNotFound;
        }

        var response = sessions
            .Select(s => new SessionResponse
            {
                SessionId = s.Id.ToString(),
                LastSendDate = s.LastSendDate.ToString(CultureInfo.InvariantCulture)
            })
            .ToList();

        return response;
    }
}