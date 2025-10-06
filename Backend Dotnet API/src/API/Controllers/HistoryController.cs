using Application.DTOs.ChatHistory;
using Application.DTOs.Session;
using Application.Handlers.ChatHistory.SearchByIdAgent;
using Application.Handlers.Session.SearchByIdAgent;
using Application.Handlers.Session.SearchTwilioByIdAgent;
using Domain.Enums;
using ErrorOr;
using GemelliApi.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers;

[Route("api/{module}")]
[Authorize(Policy = "ModuleAccessPolicy")]
public class ChatHistoryController : MainController
{
    private readonly SearchChatHistoryByIdAgentHandler _searchChatHistoryByIdAgentHandler;
    private readonly SearchSessionByIdAgentHandler _searchSessionByIdAgentHandler;
    private readonly SearchSessionTwilioByIdAgentHandler _searchSessionTwilioByIdAgentHandler;

    public ChatHistoryController(
        SearchChatHistoryByIdAgentHandler searchChatHistoryByIdAgentHandler,
        SearchSessionByIdAgentHandler searchSessionByIdAgentHandler,
        SearchSessionTwilioByIdAgentHandler searchSessionTwilioByIdAgentHandler)
    {
        _searchChatHistoryByIdAgentHandler = searchChatHistoryByIdAgentHandler;
        _searchSessionByIdAgentHandler = searchSessionByIdAgentHandler;
        _searchSessionTwilioByIdAgentHandler = searchSessionTwilioByIdAgentHandler;
    }

    [HttpGet("chat-history/{idSession:guid}")]
    [ProducesResponseType(typeof(List<ChatHistoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchByIdAgent(
        [FromRoute]
        Module module,
        Guid idSession,
        CancellationToken cancellationToken)
    {
        ErrorOr<List<ChatHistoryResponse>> result = await _searchChatHistoryByIdAgentHandler.Handle(
            new SearchChatHistoryByIdAgentRequest(idSession),
            module,
            cancellationToken
        );

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpGet("session-history/{idAgent:guid}/{idUser:guid}")]
    [ProducesResponseType(typeof(List<SessionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchByIdAgentAndIdUser(
        [FromRoute]
        Module module,
        Guid idAgent,
        Guid idUser,
        CancellationToken cancellationToken)
    {
        ErrorOr<List<SessionResponse>> result = await _searchSessionByIdAgentHandler.Handle(
            new SearchSessionByIdAgentRequest(idAgent, idUser),
            module,
            cancellationToken
        );

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpGet("session-history/{idAgent:guid}")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchTwilioByIdAgent(
        [FromRoute]
        Module module,
        Guid idAgent,
        CancellationToken cancellationToken)
    {
        ErrorOr<SessionResponse> result = await _searchSessionTwilioByIdAgentHandler.Handle(
            new SearchSessionTwilioByIdAgentRequest(idAgent),
            module,
            cancellationToken
        );

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }
}