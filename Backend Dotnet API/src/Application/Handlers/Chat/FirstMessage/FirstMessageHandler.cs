using Application.DTOs.Chat;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using System.Collections.Generic;
using System.Linq;
using AgentEntity = Domain.Entities.Agent;
using ChatHistoryEntity = Domain.Entities.ChatHistory;
using ChatSessionEntity = Domain.Entities.ChatSession;

namespace Application.Handlers.Chat.FirstMessage;

public class FirstMessageHandler : BaseHandler
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IModuleService _moduleService;
    private readonly IAgentRepository _agentRepository;
    private readonly IChatSessionRepository _chatSessionRepository;
    private readonly IChatHistoryRepository _chatHistoryRepository;
    private readonly IGemelliAIService _gemelliAIService;

    public FirstMessageHandler(
        IAuthenticationService authenticationService,
        IModuleService moduleService,
        IAgentRepository agentRepository,
        IChatSessionRepository chatSessionRepository,
        IChatHistoryRepository chatHistoryRepository,
        IGemelliAIService gemelliAIService)
    {
        _authenticationService = authenticationService;
        _moduleService = moduleService;
        _agentRepository = agentRepository;
        _chatSessionRepository = chatSessionRepository;
        _chatHistoryRepository = chatHistoryRepository;
        _gemelliAIService = gemelliAIService;
    }

    public async Task<ErrorOr<FirstMessageResponse>> Handle(
        FirstMessageRequest request,
        Module module,
        CancellationToken cancellationToken = default)
    {
        if (Validate(request, new FirstMessageRequestValidator()) is var validation && validation.Any())
        {
            return validation;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        if (!_moduleService.HasAccessToModule(user, module))
        {
            return UserErrors.ModuleAccessDenied;
        }

        AgentEntity? agent = await _agentRepository.SearchByIdAsync(request.IdAgent,
            user.Organizations.FirstOrDefault()!,
            module,
            cancellationToken);

        if (agent is null)
        {
            return AgentErrors.AgentNotFound;
        }

        //var filesForService = agent.Files
        //.Where(f => !string.IsNullOrWhiteSpace(f.Content))
        //.Select(f => (Name: f.FileName, Content: f.Content!))
        
        string teste = module.ToString();
        if (string.IsNullOrWhiteSpace(teste))
        {
            module = Module.People;
        }

        List<string> documents = agent.Files
            .Where(file => file.Id != Guid.Empty)
            .Select(file => file.Id.ToString())
            .ToList();

        ErrorOr<GemelliAIChatResponse> chatResult = await _gemelliAIService.ChatAsync(new GemelliAIChatRequest
        {
            IdAgent = agent.Id.ToString(),
            Message = request.Message,
            Module = module.ToString(),
            Organization = agent.Organization,
            AgentType = "basic",
            User = new GemelliAIChatUser
            {
                Name = user.Name,
                Email = user.Email
            },
            Preferences = new Dictionary<string, string>(),
            Documents = documents
        }, cancellationToken);

        if (chatResult.IsError)
        {
            return chatResult.Errors;
        }

        string chatTitle = string.Empty;

        if (!string.IsNullOrWhiteSpace(chatResult.Value.IdSession))
        {
            ErrorOr<string> chatTitleResult = await _gemelliAIService.GetChatTitleAsync(
                chatResult.Value.IdSession,
                cancellationToken);

            chatTitle = chatTitleResult.IsError ? string.Empty : chatTitleResult.Value;
        }

        ChatSessionEntity session = new()
        {
            IdAgent = agent.Id,
            Agent = agent,
            IdUser = user.IdUser,
            TotalInteractions = 1,
            LastSendDate = DateTime.UtcNow,
            IdSession = chatResult.Value.IdSession,
            Title = chatTitle
        };
        await _chatSessionRepository.AddAsync(session, cancellationToken);

        ChatHistoryEntity userHistory = new()
        {
            IdChatSession = session.Id,
            ChatSession = session,
            IdUser = user.IdUser,
            Content = request.Message,
            Role = RoleChat.User
        };
        await _chatHistoryRepository.AddAsync(userHistory, cancellationToken);

        ChatHistoryEntity aiHistory = new()
        {
            IdChatSession = session.Id,
            ChatSession = session,
            IdUser = user.IdUser,
            Content = chatResult.Value.MessageResponse,
            Role = RoleChat.System,
            TotalTokens = chatResult.Value.Usage?.GrandTotalUsage?.TotalTokens ?? 0
        };
        await _chatHistoryRepository.AddAsync(aiHistory, cancellationToken);

        await _chatSessionRepository.UnitOfWork.Commit();

        return new FirstMessageResponse
        {
            MessageResponse = chatResult.Value.MessageResponse,
            SessionId = session.Id.ToString(),
            ExternalSessionId = session.IdSession ?? string.Empty,
            Title = session.Title ?? string.Empty
        };
    }
}
