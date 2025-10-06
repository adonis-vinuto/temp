using System.Security.Claims;
using Application.Common.Responses;
using Application.DTOs.Knowledge;
using Application.Handlers.Knowledge.AttachToAgent;
using Application.Handlers.Knowledge.Create;
using Application.Handlers.Knowledge.CreateImportSenior;
using Application.Handlers.Knowledge.Edit;
using Application.Handlers.Knowledge.ImportExcel;
using Application.Handlers.Knowledge.Remove;
using Application.Handlers.Knowledge.Search;
using Application.Handlers.Knowledge.SearchById;
using Domain.Enums;
using ErrorOr;
using GemelliApi.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Route("api/{module}/knowledge")]
[Authorize(Policy = "ModuleAccessPolicy")]
public class KnowledgeController : MainController
{
    private readonly CreateKnowledgeHandler _createKnowledgeHandler;
    private readonly SearchKnowledgeByIdHandler _searchKnowledgeByIdHandler;
    private readonly SearchKnowledgesHandler _searchKnowledgesHandler;
    private readonly EditKnowledgeHandler _editKnowledgeHandler;
    private readonly AttachToAgentKnowledgeHandler _attachToAgentKnowledgeHandler;
    private readonly RemoveKnowledgeHandler _removeKnowledgeHandler;
    private readonly ImportExcelKnowledgeHandler _importExcelKnowledgeHandler;
    private readonly CreateImportSeniorHandler _createImportSeniorHandler;

    public KnowledgeController(CreateKnowledgeHandler createKnowledgeHandler,
    SearchKnowledgeByIdHandler searchKnowledgeByIdHandler,
    SearchKnowledgesHandler searchKnowledgesHandler, RemoveKnowledgeHandler removeKnowledgeHandler, EditKnowledgeHandler editKnowledgeHandler, AttachToAgentKnowledgeHandler attachToAgentKnowledgeHandler, ImportExcelKnowledgeHandler importExcelKnowledgeHandler, CreateImportSeniorHandler createImportSeniorHandler)
    {
        _createKnowledgeHandler = createKnowledgeHandler;
        _searchKnowledgeByIdHandler = searchKnowledgeByIdHandler;
        _searchKnowledgesHandler = searchKnowledgesHandler;
        _removeKnowledgeHandler = removeKnowledgeHandler;
        _editKnowledgeHandler = editKnowledgeHandler;
        _attachToAgentKnowledgeHandler = attachToAgentKnowledgeHandler;
        _importExcelKnowledgeHandler = importExcelKnowledgeHandler;
        _createImportSeniorHandler = createImportSeniorHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(KnowledgeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateKnowledge(
              [FromRoute] Module module,
              [FromBody] CreateKnowledgeRequest request,
              CancellationToken cancellationToken)
    {
        ErrorOr<KnowledgeResponse> result = await _createKnowledgeHandler.Handle(request, module, cancellationToken);

        return result.Match(
            Knowledge => Ok(Knowledge),
            errors => Problem(errors)
        );
    }

    [HttpPost("{idKnowledge:guid}/{idSeniorHcmConfig:guid}")]
    [ProducesResponseType(typeof(KnowledgeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateImportSenior(
            Guid idKnowledge,
            Guid idSeniorHcmConfig,
            [FromRoute] Module module,
            CancellationToken cancellationToken)
    {
        ErrorOr<KnowledgeResponse> result = await _createImportSeniorHandler.Handle(new CreateImportSeniorRequest(idKnowledge, idSeniorHcmConfig), module, cancellationToken);

        return result.Match(
            Knowledge => Ok(Knowledge),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(KnowledgeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchById([FromRoute] Module module, Guid id, CancellationToken cancellationToken)
    {
        ErrorOr<KnowledgeResponse> result = await _searchKnowledgeByIdHandler.Handle(
            new SearchKnowledgeByIdRequest(id),
            module,
            cancellationToken
        );

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<List<KnowledgeResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchKnowledges([FromRoute] Module module,
        [FromQuery] Guid? idAgent,
        CancellationToken cancellationToken,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10
        )
    {
        ErrorOr<PagedResponse<KnowledgeResponse>> result =
            await _searchKnowledgesHandler.Handle(pagina, tamanhoPagina, module, idAgent, cancellationToken);

        return result.IsError
            ? Problem(result.FirstError.Description)
            : Ok(result.Value);

    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Editar(Guid id, [FromRoute] Module module, [FromBody] EditKnowledgeRequest request, CancellationToken cancellationToken)
    {
        request.Id = id;

        ErrorOr<KnowledgeResponse> result = await _editKnowledgeHandler.Handle(request, module, cancellationToken);

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpPut("{id:guid}/attach")]
    public async Task<IActionResult> AnexarAoAgente(Guid id, [FromRoute] Module module, [FromBody] AttachToAgentKnowledgeRequest attachToAgentKnowledgeRequest, CancellationToken cancellationToken)
    {
        attachToAgentKnowledgeRequest.Id = id;

        ErrorOr<KnowledgeResponse> result = await _attachToAgentKnowledgeHandler.Handle(attachToAgentKnowledgeRequest, module, cancellationToken);

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id, [FromRoute] Module module, CancellationToken cancellationToken)
    {
        ErrorOr<KnowledgeResponse> result = await _removeKnowledgeHandler.Handle(new RemoveKnowledgeRequest(id), module, cancellationToken);

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

    [HttpPost("{idKnowledge:guid}/import-excel")]
    [ProducesResponseType(typeof(KnowledgeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportExcel(
            [FromRoute] Module module,
            [FromRoute] Guid idKnowledge,
            [FromForm] ImportExcelKnowledgeRequest request,
            CancellationToken cancellationToken)
    {
        request.IdKnowledge = idKnowledge;

        ErrorOr<KnowledgeResponse> result = await _importExcelKnowledgeHandler.Handle(request, module, cancellationToken);

        return result.Match(
            Knowledge => Ok(Knowledge),
            errors => Problem(errors)
        );
    }
}