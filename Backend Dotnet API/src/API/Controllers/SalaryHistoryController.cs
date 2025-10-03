using Application.Common.Responses;
using Application.DTOs.Agent;
using Application.DTOs.SalaryHistory;
using Application.Handlers.Agent.Remove;
using Application.Handlers.People.SalaryHistory.Create;
using Application.Handlers.People.SalaryHistory.Edit;
using Application.Handlers.People.SalaryHistory.Remove;
using Application.Handlers.People.SalaryHistory.SearchByIdAndIdKnowledge;
using Application.Handlers.People.SalaryHistory.SearchByIdKnowledge;
using Domain.Entities;
using Domain.Enums;
using ErrorOr;
using GemelliApi.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Route("api/people/salary-history")]
[Authorize(Roles = "People")]
public class SalaryHistoryController : MainController
{
    private readonly SearchSalaryHistoryIdKnowledgeHandler _searchSalariesHistoriesByKnowledgeHandler;
    private readonly SearchSalaryHistoryIdAndIdKnowledgeHandler _searchSalariesHistoriesByIdAndKnowledgeHandler;
    private readonly CreateSalaryHistoryHandler _createSalaryHistoryHandler;
    private readonly EditSalaryHistoryHandler _editSalaryHistoryHandler;
    private readonly RemoveSalaryHistoryHandler _removeSalaryHistoryHandler;

    public SalaryHistoryController(SearchSalaryHistoryIdKnowledgeHandler searchSalaryHistoryHandler, CreateSalaryHistoryHandler createSalaryHistoryHandler, SearchSalaryHistoryIdAndIdKnowledgeHandler searchSalaryHistoryIdAndIdKnowledgeHandler, EditSalaryHistoryHandler editSalaryHistoryHandler, RemoveSalaryHistoryHandler removeSalaryHistoryHandler)
    {
        _searchSalariesHistoriesByKnowledgeHandler = searchSalaryHistoryHandler;
        _searchSalariesHistoriesByIdAndKnowledgeHandler = searchSalaryHistoryIdAndIdKnowledgeHandler;
        _createSalaryHistoryHandler = createSalaryHistoryHandler;
        _editSalaryHistoryHandler = editSalaryHistoryHandler;
        _removeSalaryHistoryHandler = removeSalaryHistoryHandler;
    }

    [HttpGet("{idKnowledge:guid}")]
    [ProducesResponseType(typeof(PagedResponse<List<SalaryHistory>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchSalariesHistoriesByKnowledge(Module module,
        Guid idKnowledge,
        CancellationToken cancellationToken,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10
        )
    {
        ErrorOr<PagedResponse<SalaryHistoryResponse>> result =
            await _searchSalariesHistoriesByKnowledgeHandler.Handle(idKnowledge, module, pagina, tamanhoPagina, cancellationToken);

        return result.IsError
            ? Problem(result.FirstError.Description)
            : Ok(result.Value);

    }

    [HttpGet("{idKnowledge:guid}/{idSalaryHistory}")]
    [ProducesResponseType(typeof(PagedResponse<SalaryHistoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchSalariesHistoriesByIdAndIdKnowledge(Module module,
        Guid idSalaryHistory,
        Guid idKnowledge,
        CancellationToken cancellationToken
        )
    {
        ErrorOr<SalaryHistoryResponse> result =
            await _searchSalariesHistoriesByIdAndKnowledgeHandler.Handle(idSalaryHistory, idKnowledge, module, cancellationToken);

        return result.IsError
            ? Problem(result.FirstError.Description)
            : Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SalaryHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAgent(Module module,
        [FromBody] CreateSalaryHistoryRequest request,
        CancellationToken cancellationToken)
    {
        ErrorOr<SalaryHistoryResponse> result = await _createSalaryHistoryHandler.Handle(module, request, cancellationToken);

        return result.Match(
            salary => Ok(salary),
            errors => Problem(errors)
        );
    }

    [HttpPut("{idKnowledge:guid}/{idSalaryHistory:guid}")]
    [ProducesResponseType(typeof(SalaryHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAgent(Module module,
        Guid idSalaryHistory,
        Guid idKnowledge,
        [FromBody] EditSalaryHistoryRequest request,
        CancellationToken cancellationToken)
    {
        ErrorOr<SalaryHistoryResponse> result = await _editSalaryHistoryHandler.Handle(idSalaryHistory, idKnowledge, module, request, cancellationToken);

        return result.Match(
            salary => Ok(salary),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{idKnowledge:guid}/{idSalaryHistory:guid}")]
    [ProducesResponseType(typeof(SalaryHistoryResponse), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(Guid idSalaryHistory, Guid idKnowledge, [FromRoute] Module module, CancellationToken cancellationToken)
    {
        ErrorOr<SalaryHistoryResponse> result = await _removeSalaryHistoryHandler.Handle(new RemoveSalaryHistoryRequest(idSalaryHistory, idKnowledge), module, cancellationToken);

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