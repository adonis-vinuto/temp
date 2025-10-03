using System.Security.Claims;
using Application.Common.Responses;
using Application.DTOs.Agent;
using Application.Handlers.Agent.Create;
using Application.Handlers.Agent.Edit;
using Application.Handlers.Agent.Remove;
using Application.Handlers.Agent.Search;
using Application.Handlers.Agent.SearchById;
using Domain.Enums;
using ErrorOr;
using GemelliApi.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Route("api/{module}/agents")]
[Authorize(Policy = "ModuleAccessPolicy")]
public class AgentController : MainController
{
    private readonly CreateAgentHandler _createAgentHandler;
    private readonly SearchAgentByIdHandler _searchAgentByIdHandler;
    private readonly SearchAgentsHandler _searchAgentsHandler;
    private readonly EditAgentHandler _editAgentHandler;
    private readonly RemoveAgentHandler _removeAgentHandler;

    public AgentController(CreateAgentHandler createAgentHandler,
    SearchAgentByIdHandler searchAgentByIdHandler,
    SearchAgentsHandler searchAgentsHandler,
    EditAgentHandler editAgentHandler, RemoveAgentHandler removeAgentHandler)
    {
        _createAgentHandler = createAgentHandler;
        _searchAgentByIdHandler = searchAgentByIdHandler;
        _searchAgentsHandler = searchAgentsHandler;
        _editAgentHandler = editAgentHandler;
        _removeAgentHandler = removeAgentHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AgentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAgent(
        [FromRoute] Module module,
        [FromBody] CreateAgentRequest request, 
        CancellationToken cancellationToken)
    {
        ErrorOr<AgentResponse> result = await _createAgentHandler.Handle(request, module, cancellationToken);

        return result.Match(
            agent => Ok(agent),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AgentByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchById([FromRoute] Module module, Guid id, CancellationToken cancellationToken)
    {
        ErrorOr<AgentByIdResponse> result = await _searchAgentByIdHandler.Handle(
            new SearchAgentByIdRequest(id),
            module,
            cancellationToken
        );

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<List<AgentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchAgents([FromRoute] Module module,
        CancellationToken cancellationToken,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10)
    {
        ErrorOr<PagedResponse<AgentResponse>> result =
            await _searchAgentsHandler.Handle(pagina, tamanhoPagina, module, cancellationToken);

        return result.IsError
            ? Problem(result.FirstError.Description)
            : Ok(result.Value);
        
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AgentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Editar(
        Guid id,
        [FromRoute] Module module,
        [FromBody] EditAgentRequest request,
        CancellationToken cancellationToken)
    {
        request.Id = id;

        ErrorOr<AgentResponse> result = await _editAgentHandler.Handle(request, module, cancellationToken);

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(AgentResponse), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(Guid id, [FromRoute] Module module, CancellationToken cancellationToken)
    {
        ErrorOr<AgentResponse> result = await _removeAgentHandler.Handle(new RemoveAgentRequest(id), module, cancellationToken);

        if (result.IsError)
        {
            return Problem(
                title: result.FirstError.Code,
                detail: result.FirstError.Description,
                statusCode: MapToHttpStatus(result.FirstError.Type)
            );
        }
        
        return NoContent();
    }
}