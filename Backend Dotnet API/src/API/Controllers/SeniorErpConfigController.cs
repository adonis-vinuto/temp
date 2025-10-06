using Application.Common.Responses;
using Application.DTOs.SeniorErpConfig;
using Application.Handlers.SeniorErpConfig.AttachToAgent;
using Application.Handlers.SeniorErpConfig.Create;
using Application.Handlers.SeniorErpConfig.Edit;
using Application.Handlers.SeniorErpConfig.Remove;
using Application.Handlers.SeniorErpConfig.Search;
using Application.Handlers.SeniorErpConfig.SearchById;
using Domain.Enums;
using ErrorOr;
using GemelliApi.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/{module}/senior-erp-config")]
[Authorize(Policy = "ModuleAccessPolicy")]

public class SeniorErpConfigController : MainController
{
    private readonly CreateSeniorErpConfigHandler _createHandler;
    private readonly EditSeniorErpConfigHandler _editSeniorErpConfigHandler;
    private readonly SearchSeniorErpConfigHandler _searchSeniorErpConfigHandler;
    private readonly RemoveSeniorErpConfigHandler _removeSeniorErpConfigHandler;
    private readonly SearchByIdSeniorErpConfigHandler _searchByIdSeniorErpConfigHandler;
    private readonly AttachToAgentSeniorErpConfigHandler _attachToAgentSeniorErpConfigHandler;

    public SeniorErpConfigController(
        CreateSeniorErpConfigHandler createHandler,
        EditSeniorErpConfigHandler editSeniorErpConfigHandler,
        SearchSeniorErpConfigHandler searchSeniorErpConfigHandler,
        RemoveSeniorErpConfigHandler removeSeniorErpConfigHandler,
        SearchByIdSeniorErpConfigHandler searchByIdSeniorErpConfigHandler,
        AttachToAgentSeniorErpConfigHandler attachToAgentSeniorErpConfigHandler)
    {
        _createHandler = createHandler;
        _editSeniorErpConfigHandler = editSeniorErpConfigHandler;
        _searchSeniorErpConfigHandler = searchSeniorErpConfigHandler;
        _removeSeniorErpConfigHandler = removeSeniorErpConfigHandler;
        _searchByIdSeniorErpConfigHandler = searchByIdSeniorErpConfigHandler;
        _attachToAgentSeniorErpConfigHandler = attachToAgentSeniorErpConfigHandler;
    }

    [HttpPost]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(SeniorErpConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSeniorErpConfig(
        [FromBody] CreateSeniorErpConfigRequest request,
        CancellationToken cancellationToken)
    {
        ErrorOr<SeniorErpConfigResponse> result = await _createHandler.Handle(request, cancellationToken);

        return result.Match(
            ok => Ok(ok),
            errors => Problem(detail: string.Join("; ", errors.Select(e => e.Description)))
        );
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(SeniorErpConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchByIdSenior(Guid id, CancellationToken cancellationToken)
    {
        ErrorOr<SeniorErpConfigResponse> result = await _searchByIdSeniorErpConfigHandler.Handle(
            new SearchByIdSeniorErpConfigRequest(id), cancellationToken);

        return result.IsError
            ? Problem(
                title: result.FirstError.Code,
                detail: result.FirstError.Description
              )
            : Ok(result.Value);
    }

    [HttpGet]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(PagedResponse<List<SeniorErpConfigResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchSeniorErpConfig(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        ErrorOr<PagedResponse<SeniorErpConfigResponse>> result =
            await _searchSeniorErpConfigHandler.Handle(pagina, tamanhoPagina, cancellationToken);

        return result.IsError
            ? Problem(result.FirstError.Description)
            : Ok(result.Value);
        
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(SeniorErpConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditarSeniorErpConfig(
        [FromRoute] Guid id,
        [FromBody] EditSeniorErpConfigRequest request,
        CancellationToken cancellationToken)
    {

        request.Id = id;

        ErrorOr<SeniorErpConfigResponse> result = await _editSeniorErpConfigHandler.Handle(request, cancellationToken);

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description)
            : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "owner")]
    public async Task<IActionResult> RemoverSeniorErpConfig([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        ErrorOr<SeniorErpConfigResponse> result = await _removeSeniorErpConfigHandler.Handle(
            new RemoveSeniorErpConfigRequest(id), cancellationToken);

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
    
    [HttpPut("{id:guid}/attach")]
    public async Task<IActionResult> AnexarAoAgente(Guid id, [FromRoute] Module module, [FromBody] AttachToAgentSeniorErpConfigRequest attachToAgentSeniorErpConfigRequest, CancellationToken cancellationToken)
    {
        attachToAgentSeniorErpConfigRequest.Id = id;

        ErrorOr<SeniorErpConfigResponse> result = await _attachToAgentSeniorErpConfigHandler.Handle(attachToAgentSeniorErpConfigRequest, module, cancellationToken);

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }
}
