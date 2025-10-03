using Application.Common.Responses;
using Application.Contracts.Response;
using Application.DTOs.File;
using Application.Handlers.File.AttachToAgent;
using Application.Handlers.File.DetachFromAgent;
using Application.Handlers.File.Create;
using Application.Handlers.File.Remove;
using Application.Handlers.File.Search;
using Application.Handlers.File.SearchById;
using Domain.Enums;
using ErrorOr;
using GemelliApi.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Route("api/{module}/file")]
[Authorize(Policy = "ModuleAccessPolicy")]
public class FileController : MainController
{
    private readonly CreateFileHandler _createFileHandler;
    private readonly SearchFileByIdHandler _searchFileByIdHandler;
    private readonly SearchFilesHandler _searchFilesHandler;
    private readonly AttachToAgentFileHandler _attachToAgentFileHandler;
    private readonly RemoveFileHandler _removeFileHandler;
    private readonly DetachFromAgentFileHandler _detachFromAgentFileHandler;

    public FileController(CreateFileHandler createFileHandler,
        SearchFileByIdHandler searchFileByIdHandler,
        SearchFilesHandler searchFilesHandler,
        RemoveFileHandler removeFileHandler,
        AttachToAgentFileHandler attachToAgentFileHandler,
        DetachFromAgentFileHandler detachFromAgentFileHandler)
    {
        _createFileHandler = createFileHandler;
        _searchFileByIdHandler = searchFileByIdHandler;
        _searchFilesHandler = searchFilesHandler;
        _removeFileHandler = removeFileHandler;
        _attachToAgentFileHandler = attachToAgentFileHandler;
        _detachFromAgentFileHandler = detachFromAgentFileHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(FileResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFile(
              [FromRoute] Module module,
              [FromForm] CreateFileRequest request,
              CancellationToken cancellationToken)
    {
        ErrorOr<FileResponseModel> result = await _createFileHandler.Handle(request, module, cancellationToken);

        return result.Match(
            File => Ok(File),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(FileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchById([FromRoute] Module module, Guid id, CancellationToken cancellationToken)
    {
        ErrorOr<FileResponse> result = await _searchFileByIdHandler.Handle(
            new SearchFileByIdRequest(id),
            module,
            cancellationToken
        );

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<FileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchFiles([FromRoute] Module module,
        [FromQuery] SearchFilesRequest request,
        CancellationToken cancellationToken
        )
    {
        ErrorOr<PagedResponse<FileResponse>> result =
            await _searchFilesHandler.Handle(request, module, cancellationToken);

        return result.IsError
            ? Problem(result.FirstError.Description)
            : Ok(result.Value);

    }

    [HttpPut("{id:guid}/attach")]
    public async Task<IActionResult> AnexarAoAgente(Guid id, [FromRoute] Module module, [FromBody] AttachToAgentFileRequest attachToAgentFileRequest, CancellationToken cancellationToken)
    {
        attachToAgentFileRequest.IdFile = id;

        ErrorOr<FileResponse> result = await _attachToAgentFileHandler.Handle(attachToAgentFileRequest, module, cancellationToken);

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpDelete("{id:guid}/attach/{idAgent:guid}")]
    public async Task<IActionResult> DesanexarDoAgente(Guid id, Guid idAgent, [FromRoute] Module module, CancellationToken cancellationToken)
    {
        var request = new DetachFromAgentFileRequest(idAgent) { IdFile = id };
        ErrorOr<FileResponse> result = await _detachFromAgentFileHandler.Handle(request, module, cancellationToken);

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id, [FromRoute] Module module, CancellationToken cancellationToken)
    {
        ErrorOr<FileResponse> result = await _removeFileHandler.Handle(new RemoveFileRequest(id), module, cancellationToken);

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