using Application.Common.Responses;
using Application.DTOs.Employee;
using Application.Handlers.People.Employee.Create;
using Application.Handlers.People.Employee.Edit;
using Application.Handlers.People.Employee.Remove;
using Application.Handlers.People.Employee.Search;
using Application.Handlers.People.Employee.SearchById;
using ErrorOr;
using GemelliApi.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/people/employee")]
[Authorize(Roles = "People")]
public class EmployeeController : MainController
{
    private readonly CreateEmployeeHandler _createEmployeeHandler;
    private readonly SearchEmployeeByIdHandler _searchEmployeeByIdHandler;
    private readonly SearchEmployeeHandler _searchEmployeesHandler;
    private readonly EditEmployeeHandler _editEmployeeHandler;
    private readonly RemoveEmployeeHandler _removeEmployeeHandler;

    public EmployeeController(
        CreateEmployeeHandler createEmployeeHandler,
        SearchEmployeeByIdHandler searchEmployeeByIdHandler,
        SearchEmployeeHandler searchEmployeesHandler,
        EditEmployeeHandler editEmployeeHandler,
        RemoveEmployeeHandler removeEmployeeHandler)
    {
        _createEmployeeHandler = createEmployeeHandler;
        _searchEmployeeByIdHandler = searchEmployeeByIdHandler;
        _searchEmployeesHandler = searchEmployeesHandler;
        _editEmployeeHandler = editEmployeeHandler;
        _removeEmployeeHandler = removeEmployeeHandler;
    }

    [HttpPost("{idKnowledge:guid}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEmployee(
        [FromRoute] Guid idKnowledge,
        [FromBody] CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        request.IdKnowledge = idKnowledge;

        ErrorOr<EmployeeResponse> result = await _createEmployeeHandler.Handle(request, cancellationToken);

        return result.Match(
            employee => Ok(employee),
            errors => Problem(errors)
        );
    }

    [HttpGet("{idKnowledge:guid}/{idEmployee:guid}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchById([FromRoute] Guid idKnowledge, [FromRoute] string idEmployee, CancellationToken cancellationToken)
    {
        ErrorOr<EmployeeResponse> result = await _searchEmployeeByIdHandler.Handle(
            new SearchEmployeeByIdRequest(idKnowledge, idEmployee),
            cancellationToken
        );

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpGet("{idKnowledge:guid}")]
    [ProducesResponseType(typeof(PagedResponse<List<EmployeeResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchEmployees(
        [FromRoute] Guid idKnowledge,
        CancellationToken cancellationToken,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10)
    {
        ErrorOr<PagedResponse<EmployeeResponse>> result =
            await _searchEmployeesHandler.Handle(idKnowledge, pagina, tamanhoPagina, cancellationToken);

        return result.IsError
            ? Problem(result.FirstError.Description)
            : Ok(result.Value);

    }

    [HttpPut("{idKnowledge:guid}/{idEmployee:guid}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Editar(
        [FromRoute] Guid idKnowledge,
        [FromRoute] string idEmployee,
        [FromBody] EditEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        request.Id = idEmployee;

        ErrorOr<EmployeeResponse> result = await _editEmployeeHandler.Handle(request, idKnowledge, cancellationToken);

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpDelete("{idKnowledge:guid}/{idEmployee:guid}")]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remover(
        [FromRoute] Guid idKnowledge,
        [FromRoute] string idEmployee,
        CancellationToken cancellationToken)
    {
        ErrorOr<EmployeeResponse> result = await _removeEmployeeHandler.Handle(new RemoveEmployeeRequest(idKnowledge, idEmployee), cancellationToken);

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