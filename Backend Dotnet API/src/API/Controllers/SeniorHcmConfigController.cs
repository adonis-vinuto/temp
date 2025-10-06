using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Responses;
using Application.DTOs.Knowledge;
using Application.DTOs.SeniorHcmConfig;
using Application.Handlers.SeniorErpConfig.AttachToAgent;
using Application.Handlers.SeniorHcmConfig.AttachToAgent;
using Application.Handlers.SeniorHcmConfig.Create;
using Application.Handlers.SeniorHcmConfig.Edit;
using Application.Handlers.SeniorHcmConfig.Remove;
using Application.Handlers.SeniorHcmConfig.Search;
using Application.Handlers.SeniorHcmConfig.SearchById;
using Domain.Enums;
using ErrorOr;
using GemelliApi.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

[Route("api/{module}/senior-hcm-config")]
[Authorize(Policy = "ModuleAccessPolicy")]
public class SeniorHcmConfigController : MainController
{
    private readonly CreateSeniorHcmConfigHandler _createSeniorHcmConfigHandler;
    private readonly EditSeniorHcmConfigHandler _editSeniorHcmConfigHandler;
    private readonly SearchSeniorHcmConfigHandler _searchSeniorHcmConfigHandler;
    private readonly SearchByIdSeniorHcmConfigHandler _searchByIdSeniorHcmConfigHandler;
    private readonly RemoveSeniorHcmConfigHandler _removeSeniorHcmConfigHandler;
    private readonly AttachToAgentSeniorHcmConfigHandler _attachToAgentSeniorHcmConfigHandler;

    public SeniorHcmConfigController(
        CreateSeniorHcmConfigHandler createSeniorHcmConfigHandler,
        EditSeniorHcmConfigHandler editSeniorHcmConfigHandler,
        SearchSeniorHcmConfigHandler searchSeniorHcmConfigHandler,
        SearchByIdSeniorHcmConfigHandler searchByIdSeniorHcmConfigHandler,
        RemoveSeniorHcmConfigHandler removeSeniorHcmConfigHandler,
        AttachToAgentSeniorHcmConfigHandler attachToAgentSeniorHcmConfigHandler)
    {
        _createSeniorHcmConfigHandler = createSeniorHcmConfigHandler;
        _editSeniorHcmConfigHandler = editSeniorHcmConfigHandler;
        _searchSeniorHcmConfigHandler = searchSeniorHcmConfigHandler;
        _searchByIdSeniorHcmConfigHandler = searchByIdSeniorHcmConfigHandler;
        _removeSeniorHcmConfigHandler = removeSeniorHcmConfigHandler;
        _attachToAgentSeniorHcmConfigHandler = attachToAgentSeniorHcmConfigHandler;
    }

    [HttpGet]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(PagedResponse<List<SeniorHcmConfigResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchSeniorHcmConfig( 
        CancellationToken cancellationToken,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10)
    {
        ErrorOr<PagedResponse<SeniorHcmConfigResponse>> result =
            await _searchSeniorHcmConfigHandler.Handle(pagina, tamanhoPagina, cancellationToken);

        return result.IsError
            ? Problem(result.FirstError.Description)
            : Ok(result.Value);
        
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(SeniorHcmConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchByIdSenior(Guid id, CancellationToken cancellationToken)
    {
        ErrorOr<SeniorHcmConfigResponse> result = await _searchByIdSeniorHcmConfigHandler.Handle(
            new SearchByIdSeniorHcmConfigRequest(id), cancellationToken);

        return result.IsError
            ? Problem(
                title: result.FirstError.Code,
                detail: result.FirstError.Description
              )
            : Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(SeniorHcmConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSeniorHcmConfig(
        [FromBody] CreateSeniorHcmConfigRequest request,
        CancellationToken cancellationToken)
    {
        ErrorOr<SeniorHcmConfigResponse> result =
            await _createSeniorHcmConfigHandler.Handle(request, cancellationToken);

        return result.Match(
            ok => Ok(ok),
            errors => Problem(detail: string.Join("; ", errors.Select(e => e.Description)))
        );
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(SeniorHcmConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditarSeniorHcmConfig(
        [FromRoute] Guid id,
        [FromBody] EditSeniorHcmConfigRequest request,
        CancellationToken cancellationToken)
    {
        request.Id = id;

        ErrorOr<SeniorHcmConfigResponse> result = await _editSeniorHcmConfigHandler.Handle(request, cancellationToken);

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description)
            : Ok(result.Value);
    }
    
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "owner")]
    public async Task<IActionResult> RemoverSeniorErpConfig([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        ErrorOr<SeniorHcmConfigResponse> result = await _removeSeniorHcmConfigHandler.Handle(new RemoveSeniorHcmConfigRequest(id), cancellationToken);
        
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
    public async Task<IActionResult> AnexarAoAgente(Guid id, [FromRoute] Module module, [FromBody] AttachToAgentSeniorHcmConfigRequest attachToAgentSeniorHcmConfigRequest, CancellationToken cancellationToken)
    {
        attachToAgentSeniorHcmConfigRequest.Id = id;

        ErrorOr<SeniorHcmConfigResponse> result = await _attachToAgentSeniorHcmConfigHandler.Handle(attachToAgentSeniorHcmConfigRequest, module, cancellationToken);

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }
}