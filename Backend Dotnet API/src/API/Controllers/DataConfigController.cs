using Application.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using GemelliApi.API.Controllers;
using Application.Handlers.DataConfig.Create;
using Application.Handlers.DataConfig.SearchById;
using Application.Handlers.DataConfig.TestConfig;
using Application.DTOs.DataConfig;
using ErrorOr;
using Application.Handlers.DataConfig.Edit;
using Application.Handlers.DataConfig.Search;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

[Route("api/data-config")]
[Authorize]
public class DataConfigController : MainController
{
    private readonly CreateDataConfigHandler _createDataConfigHandler;
    private readonly SearchDataConfigByIdHandler _searchDataConfigByIdHandler;
    private readonly SearchDataConfigsHandler _searchDataConfigsHandler;
    private readonly EditDataConfigHandler _editDataConfigHandler;
    private readonly TestDataConfigHandler _testDataConfigHandler;

    public DataConfigController(CreateDataConfigHandler createDataConfigHandler, SearchDataConfigByIdHandler searchDataConfigByIdHandler,
        SearchDataConfigsHandler searchDataConfigsHandler, EditDataConfigHandler editDataConfigHandler, TestDataConfigHandler testDataConfigHandler)
    {
        _createDataConfigHandler = createDataConfigHandler;
        _searchDataConfigByIdHandler = searchDataConfigByIdHandler;
        _searchDataConfigsHandler = searchDataConfigsHandler;
        _editDataConfigHandler = editDataConfigHandler;
        _testDataConfigHandler = testDataConfigHandler;
    }

    [HttpPost]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(DataConfigResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDataConfig([FromBody] CreateDataConfigRequest request, CancellationToken cancellationToken)
    {
        ErrorOr<DataConfigResponse> result = await _createDataConfigHandler.Handle(request, cancellationToken);

        return result.Match(
            DataConfigResponse => Ok(DataConfigResponse),
            errors => Problem(errors)
        );
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(DataConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BuscarPorId(Guid id, CancellationToken cancellationToken)
    {
        ErrorOr<DataConfigResponse> result = await _searchDataConfigByIdHandler.Handle(
            new SearchDataConfigByIdRequest(id),
            cancellationToken
        );

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpGet()]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(IEnumerable<DataConfigResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Buscar(CancellationToken cancellationToken,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10)
    {

        ErrorOr<PagedResponse<DataConfigResponse>> result = await _searchDataConfigsHandler.Handle(
            pagina, tamanhoPagina,
            cancellationToken
        );

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "owner")]
    [ProducesResponseType(typeof(DataConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Editar(
        Guid id,
        [FromBody] EditDataConfigRequest request,
        CancellationToken cancellationToken)
    {
        request.Id = id;

        ErrorOr<DataConfigResponse> result = await _editDataConfigHandler.Handle(request, cancellationToken);

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpPost("test-connection")]
    [ProducesResponseType(typeof(TestConnectionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TestConnection([FromBody] TestDataConfigRequest request)
    {
        ErrorOr<TestConnectionResponse> result = await _testDataConfigHandler.Handle(request);

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }
}