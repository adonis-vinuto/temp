using GemelliApi.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("health")]
public class HealthController : MainController
{
  [HttpGet]
  public IActionResult HealthCheck()
  {
    return Ok("Healthy");
  }
}
