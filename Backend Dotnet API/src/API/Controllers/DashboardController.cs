using Application.DTOs.Dashboard;
using Application.Handlers.Dashboard.GetDashboard;
using Domain.Enums;
using ErrorOr;
using GemelliApi.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Route("api/{module}/dashboard")]
[Authorize(Policy = "ModuleAccessPolicy")]
public class DashboardController : MainController
{
    private readonly GetDashboardHandler _getDashboardHandler;

    public DashboardController(GetDashboardHandler getDashboardHandler)
    {
        _getDashboardHandler = getDashboardHandler;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DashboardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDashboard(
        [FromRoute] Module module,
        CancellationToken cancellationToken)
    {
        ErrorOr<DashboardResponse> result = await _getDashboardHandler.Handle(module, cancellationToken);

        return result.IsError
             ? Problem(title: result.FirstError.Code, detail: result.FirstError.Description, statusCode: MapToHttpStatus(result.FirstError.Type))
             : Ok(result.Value);
    }
}
