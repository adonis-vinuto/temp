using System;
using System.Threading;
using System.Threading.Tasks;
using GemelliApi.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using ErrorOr;

using Application.Common.Responses;
using Application.DTOs.Knowledge.People.Payroll;
using Application.Handlers.People.Payroll.Create;
using Application.Handlers.People.Payroll.Edit;
using Application.Handlers.People.Payroll.Remove;
using Application.Handlers.People.Payroll.Search;
using Application.Handlers.People.Payroll.SearchById;

namespace API.Controllers;

[Route("api/people/payroll")]
[Authorize(Roles = "People")]

public class PayrollController : MainController
{
    private readonly CreatePayrollHandler _createPayrollHandler;
    private readonly SearchByIdPayrollHandler _searchByIdPayrollHandler;
    private readonly SearchPayrollHandler _searchPayrollHandler;
    private readonly EditPayrollHandler _editPayrollHandler;
    private readonly RemovePayrollHandler _removePayrollHandler;

    public PayrollController(
        CreatePayrollHandler createPayrollHandler,
        SearchByIdPayrollHandler searchByIdPayrollHandler, 
        SearchPayrollHandler searchPayrollHandler,
        EditPayrollHandler editPayrollHandler,
        RemovePayrollHandler removePayrollHandler)
    {
        _createPayrollHandler = createPayrollHandler;
        _searchByIdPayrollHandler = searchByIdPayrollHandler;
        _searchPayrollHandler = searchPayrollHandler;
        _editPayrollHandler = editPayrollHandler;
        _removePayrollHandler = removePayrollHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PayrollResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePayroll(
        [FromBody] CreatePayrollRequest request,
        CancellationToken cancellationToken)
    {
        ErrorOr<PayrollResponse> result = await _createPayrollHandler.Handle(request, cancellationToken);
        
        return result.Match(
            payroll => Ok(payroll),
            errors => Problem(errors)
        );
    }

    [HttpGet("{idKnowledge:guid}/{idPayroll:guid}")]
    [ProducesResponseType(typeof(PayrollResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchById(
        [FromRoute] Guid idKnowledge,
        [FromRoute] Guid idPayroll,
        CancellationToken cancellationToken)
    {
        var req = new SearchByIdPayrollRequest(idPayroll, idKnowledge);
        
        ErrorOr<PayrollResponse> result = await _searchByIdPayrollHandler.Handle(req, cancellationToken);

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpGet("{idKnowledge:guid}")]
    [ProducesResponseType(typeof(PayrollResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResponse<PayrollResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchPayrolls(
        [FromRoute] Guid idKnowledge,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        ErrorOr<PagedResponse<PayrollResponse>> result = await _searchPayrollHandler.Handle(idKnowledge, pagina, tamanhoPagina, cancellationToken);
        return result.IsError ? Problem(result.FirstError.Description) : Ok(result.Value);
    }

    [HttpPut("{idKnowledge:guid}/{idPayroll:guid}")]
    [ProducesResponseType(typeof(PayrollResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Edit(
        [FromRoute] Guid idKnowledge,
        [FromRoute] Guid idPayroll,
        [FromBody] EditPayrollRequest request,
        CancellationToken cancellationToken)
    {
        request.Id = idPayroll;

        ErrorOr<PayrollResponse> result = await _editPayrollHandler.Handle(request, idKnowledge, cancellationToken);

        return result.IsError
            ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
            : Ok(result.Value);
    }

    [HttpDelete("{idKnowledge:guid}/{idPayroll:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Remove(
        [FromRoute] Guid idKnowledge,
        [FromRoute] Guid idPayroll,
        CancellationToken cancellationToken)
    {
        var request = new RemovePayrollRequest(idPayroll, idKnowledge );
        
        ErrorOr<PayrollResponse> result = await _removePayrollHandler.Handle(request, cancellationToken);

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
