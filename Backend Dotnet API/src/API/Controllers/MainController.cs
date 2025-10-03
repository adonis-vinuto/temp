using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GemelliApi.API.Controllers;

[ApiController]
public class MainController : ControllerBase
{
    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
        {
            return Problem();
        }

        if (errors.TrueForAll(e => e.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        Error firstError = errors[0];

        return Problem(
            statusCode: MapToHttpStatus(firstError.Type),
            title: firstError.Code,
            detail: firstError.Description,
            instance: HttpContext?.Request?.Path
        );
    }

    private ObjectResult ValidationProblem(List<Error> errors)
    {
        var modelState = new ModelStateDictionary();

        foreach (Error error in errors)
        {
            modelState.AddModelError(error.Code, error.Description);
        }

        var problemDetails = new ValidationProblemDetails(modelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Um ou mais erros de validação ocorreram.",
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Instance = HttpContext?.Request?.Path
        };

        problemDetails.Extensions["traceId"] = HttpContext?.TraceIdentifier;

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status,
            ContentTypes = { "application/problem+json" }
        };
    }

    protected static int MapToHttpStatus(ErrorType type) =>
        type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
}
