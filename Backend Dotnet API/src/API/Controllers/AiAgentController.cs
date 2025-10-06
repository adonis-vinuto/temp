using Application.DTOs.Agent;
using Application.Handlers.Ai.Agent.SearchByIdAndOrganization;
using Domain.Enums;
using ErrorOr;
using GemelliApi.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/ai/agent/{organization}/{module}")]
public class AiAgentController : MainController
{
    private readonly SearchByIdAndOrganizationHandler _searchByIdAndOrganizationHandler;

    public AiAgentController(
        SearchByIdAndOrganizationHandler searchByIdAndOrganizationHandler)
    {
        _searchByIdAndOrganizationHandler = searchByIdAndOrganizationHandler;
    }

    [HttpGet("{idAgent:guid}")]
    [ProducesResponseType(typeof(AiAgentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AiAgentSearch(
        [FromRoute] string organization,
        [FromRoute] Module module,
        [FromRoute] Guid idAgent,
        CancellationToken cancellationToken)
    {
        ErrorOr<AiAgentResponse> result = await _searchByIdAndOrganizationHandler.Handle(
            new SearchByIdAndOrganizationRequest(idAgent),
            module,
            organization,
            cancellationToken
        );

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

}