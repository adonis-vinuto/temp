using ErrorOr;
using MapsterMapper;
using Application.DTOs.ChatHistory;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using System.Collections.Generic;

namespace Application.Handlers.ChatHistory.SearchByIdAgent;

public class SearchChatHistoryByIdAgentHandler : BaseHandler
{
    private readonly IChatHistoryRepository _chatHistoryRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public SearchChatHistoryByIdAgentHandler(
        IChatHistoryRepository chatHistoryRepository,
        IAuthenticationService authenticationService,
        IMapper mapper)
    {
        _chatHistoryRepository = chatHistoryRepository;
        _authenticationService = authenticationService;
        _mapper = mapper;
    }

    public async Task<ErrorOr<List<ChatHistoryResponse>>> Handle(
        SearchChatHistoryByIdAgentRequest request,
        Module module,
        CancellationToken cancellationToken = default)
    {
        if (Validate(request, new SearchChatHistoryByIdAgentRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        List<Domain.Entities.ChatHistory> chatHistory = await _chatHistoryRepository.GetHistoryBySessionAsync(
            request.IdSession,
            user.IdUser,
            module,
            cancellationToken);

        if (chatHistory is null || !chatHistory.Any())
        {
            return ChatHistoryErrors.ChatHistoryNotFound;
        }

        return _mapper.Map<List<ChatHistoryResponse>>(chatHistory);
    }
}