using Application.Common.Responses;
using Application.DTOs.TwilioConfig;
using Application.Handlers.TwilioConfig.Create;
using Application.Handlers.TwilioConfig.Edit;
using Application.Handlers.TwilioConfig.Remove;
using Application.Handlers.TwilioConfig.Search;
using Application.Handlers.TwilioConfig.SearchById;
using Domain.Enums;
using ErrorOr;
using GemelliApi.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/{module}/twilio-config")]
[Authorize(Policy = "ModuleAccessPolicy")]
public class TwilioConfigController : MainController
{
    private readonly CreateTwilioConfigHandler _createTwilioConfigHandler;
    private readonly SearchTwilioConfigByIdHandler _searchTwilioConfigByIdHandler;
    private readonly SearchTwilioConfigsHandler _searchTwilioConfigsHandler;
    private readonly EditTwilioConfigHandler _editTwilioConfigHandler;
    private readonly RemoveTwilioConfigHandler _removeTwilioConfigHandler;

    public TwilioConfigController(
        CreateTwilioConfigHandler createTwilioConfigHandler,
        SearchTwilioConfigByIdHandler searchTwilioConfigByIdHandler,
        SearchTwilioConfigsHandler searchTwilioConfigsHandler,
        EditTwilioConfigHandler editTwilioConfigHandler,
        RemoveTwilioConfigHandler removeTwilioConfigHandler)
    {
        _createTwilioConfigHandler = createTwilioConfigHandler;
        _searchTwilioConfigByIdHandler = searchTwilioConfigByIdHandler;
        _searchTwilioConfigsHandler = searchTwilioConfigsHandler;
        _editTwilioConfigHandler = editTwilioConfigHandler;
        _removeTwilioConfigHandler = removeTwilioConfigHandler;
    }

    [HttpPost]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(TwilioConfigResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTwilioConfig([FromBody] CreateTwilioConfigRequest request, [FromRoute] Module module, CancellationToken cancellationToken)
    {
        ErrorOr<TwilioConfigResponse> result = await _createTwilioConfigHandler.Handle(request, module, cancellationToken);

        return result.Match(
            twilioConfigResponse => Ok(twilioConfigResponse),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(TwilioConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchById(
        Guid id,
        [FromRoute] Module module,
        CancellationToken cancellationToken)
    {
        ErrorOr<TwilioConfigResponse> result = await _searchTwilioConfigByIdHandler.Handle(
            new SearchTwilioConfigByIdRequest(id),
            module,
            cancellationToken
        );

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpGet]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(PagedResponse<TwilioConfigResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Search(
        [FromRoute] Module module,
        CancellationToken cancellationToken,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10)
    {
        ErrorOr<PagedResponse<TwilioConfigResponse>> result = await _searchTwilioConfigsHandler.Handle(
            pagina,
            tamanhoPagina,
            module,
            cancellationToken
        );

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(TwilioConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditTwilioConfig(
        Guid id,
        [FromBody] EditTwilioConfigRequest request,
        [FromRoute] Module module,
        CancellationToken cancellationToken)
    {
        request.Id = id;

        ErrorOr<TwilioConfigResponse> result = await _editTwilioConfigHandler.Handle(request, module, cancellationToken);

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(TwilioConfigResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTwilioConfig(
        Guid id,
        [FromRoute] Module module,
        CancellationToken cancellationToken)
    {
        ErrorOr<TwilioConfigResponse> result = await _removeTwilioConfigHandler.Handle(
            new RemoveTwilioConfigRequest(id),
            module,
            cancellationToken
        );

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