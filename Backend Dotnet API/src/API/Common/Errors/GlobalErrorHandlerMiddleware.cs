using System.Security.Claims;
using System.Text.Json;
using ErrorOr;
using API.Common.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Refit;
using Domain.Common;
using GemelliApi.Domain.Exceptions;

namespace API.Common.Errors;

public record GlobalErrorHandlerMiddleware(
    RequestDelegate Next,
    IOptions<ApiBehaviorOptions> Options,
    ILogger<GlobalErrorHandlerMiddleware> Logger
)
{
    private readonly RequestDelegate _next = Next;

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next.Invoke(httpContext);
        }
        catch (DomainValidationException ex)
        {
            await HandleDomainValidationExceptionAsync(httpContext, ex);
        }
        catch (DomainException ex)
        {
            await HandleDomainExceptionAsync(httpContext, ex);
        }
        catch (CustomException customException)
        {
            await HandleCustomExceptionAsync(httpContext, customException);
        }
        catch (ApiException ex)
        {
            await HandleCustomExceptionAsync(httpContext, new CustomException(ex.Message, ex));
        }
        catch (Exception exception)
        {
            LogExceptionError(httpContext, exception, null);
            await HandleErrorAsync(httpContext, exception);
        }
    }

    private async Task HandleDomainExceptionAsync(HttpContext httpContext, DomainException ex)
    {
        LogExceptionError(httpContext, ex);

        var error = Error.Custom(
            type: (int)ErrorType.Validation,
            code: ex.ErrorCode ?? "DOMAIN_RULE_VIOLATION",
            description: ex.Message
        );

        httpContext.Items[HttpContextItemKeys.Erros] = new List<Error> { error };
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        httpContext.Response.ContentType = "application/problem+json";

        Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails = new GemelliApiProblemDetailsFactory(Options)
            .CreateProblemDetails(
                httpContext,
                StatusCodes.Status400BadRequest,
                title: "Violação de regra de negócio",
                detail: ex.Message,
                instance: httpContext.Request.Path
            );

        if (!string.IsNullOrEmpty(ex.ErrorCode))
        {
            problemDetails.Extensions["errorCode"] = ex.ErrorCode;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails);
    }

    private async Task HandleDomainValidationExceptionAsync(HttpContext httpContext, DomainValidationException ex)
    {
        LogExceptionError(httpContext, ex);

        var errors = ex.Errors
            .SelectMany(kvp => kvp.Value.Select(value => Error.Validation(
                code: kvp.Key,
                description: value
            )))
            .ToList();

        httpContext.Items[HttpContextItemKeys.Erros] = errors;
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        httpContext.Response.ContentType = "application/problem+json";

        Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails = new GemelliApiProblemDetailsFactory(Options)
            .CreateProblemDetails(
                httpContext,
                StatusCodes.Status400BadRequest,
                title: "Erros de validação de domínio",
                detail: "Verifique os erros para mais detalhes",
                instance: httpContext.Request.Path
            );

        problemDetails.Extensions["errors"] = ex.Errors;

        await httpContext.Response.WriteAsJsonAsync(problemDetails);
    }

    private async Task HandleErrorAsync(HttpContext httpContext, Exception exception)
    {
        var erro = Error.Unexpected("Erro de servidor", exception.Message);
        int httpStatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Items[HttpContextItemKeys.Erros] = new List<Error> { erro };
        httpContext.Response.StatusCode = httpStatusCode;
        httpContext.Response.ContentType = "application/problem+json";

        Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails = new GemelliApiProblemDetailsFactory(Options)
            .CreateProblemDetails(httpContext, httpStatusCode,
                title: "Erro de servidor",
                detail: "Erro genérico, consulte os administradores.");

        await httpContext.Response.WriteAsJsonAsync(problemDetails);
    }

    private void LogExceptionError(HttpContext httpContext, Exception exception, IDictionary<string, object>? additionalData = null)
    {
        HttpRequest request = httpContext.Request;

        // Objeto estruturado para o Serilog
        var logData = new
        {
            User = new
            {
                Id = httpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous",
                IsAuthenticated = httpContext.User?.Identity?.IsAuthenticated
            },
            Request = new
            {
                Path = request.Path,
                Method = request.Method,
                QueryString = request.QueryString.Value,
                Headers = request.Headers
                    .Where(h => h.Key == "Host" || h.Key == "User-Agent")
                    .ToDictionary(h => h.Key, h => h.Value.ToString())
            },
            Connection = new
            {
                RemoteIp = httpContext.Connection.RemoteIpAddress?.ToString(),
                LocalIp = httpContext.Connection.LocalIpAddress?.ToString()
            },
            Exception = new
            {
                Type = exception.GetType().Name,
                Message = exception.Message,
                InnerException = exception.InnerException?.Message
            },
            AdditionalData = additionalData
        };

        Logger.LogError(
            exception,
            "Erro de servidor: {@Msg} | {@LogData}",
            exception.Message, logData);
    }

    private async Task HandleCustomExceptionAsync(HttpContext httpContext, CustomException customException)
    {
        try
        {
            var ex = customException.InnerException as ApiException;
            string? errorContent = ex?.Content;
            string mensagem = ExtractErrorMessage(errorContent) ?? customException.Message;

            LogExceptionError(httpContext, new Exception($"Erro: {mensagem}", customException), customException.AdditionalData);

            await HandleErrorAsync(
                httpContext,
                new Exception($"Erro: {mensagem}", customException));
        }
        catch (JsonException jsonEx)
        {
            Logger.LogError(jsonEx, "Falha ao desserializar resposta");
            await HandleErrorAsync(httpContext, jsonEx);
        }
    }

    private static string? ExtractErrorMessage(string? jsonContent)
    {
        if (string.IsNullOrWhiteSpace(jsonContent))
        {
            return null;
        }

        try
        {
            using var doc = JsonDocument.Parse(jsonContent);
            JsonElement root = doc.RootElement;

            // 1. Tenta obter mensagem direta (primeiro formato)
            if (root.TryGetProperty("Mensagem", out JsonElement mensagemProp) && mensagemProp.ValueKind == JsonValueKind.String)
            {
                return mensagemProp.GetString();
            }

            if (root.TryGetProperty("message", out JsonElement messageProp) && messageProp.ValueKind == JsonValueKind.String)
            {
                return messageProp.GetString();
            }

            // 2. Tenta obter mensagem do array de erros (segundo formato)
            if (root.TryGetProperty("Erros", out JsonElement errosProp) &&
                errosProp.ValueKind == JsonValueKind.Array &&
                errosProp.GetArrayLength() > 0)
            {
                JsonElement primeiroErro = errosProp[0];
                if (primeiroErro.TryGetProperty("Descricao", out JsonElement descricaoProp) &&
                    descricaoProp.ValueKind == JsonValueKind.String)
                {
                    return descricaoProp.GetString();
                }

                if (primeiroErro.TryGetProperty("Mensagem", out JsonElement erroMensagemProp) &&
                    erroMensagemProp.ValueKind == JsonValueKind.String)
                {
                    return erroMensagemProp.GetString();
                }
            }

            // 3. Tenta encontrar qualquer propriedade que possa conter mensagem
            foreach (JsonProperty prop in root.EnumerateObject())
            {
                if ((prop.Name.Contains("Mensagem", StringComparison.OrdinalIgnoreCase) ||
                    prop.Name.Contains("Message", StringComparison.OrdinalIgnoreCase) ||
                    prop.Name.Contains("Descricao", StringComparison.OrdinalIgnoreCase) ||
                    prop.Name.Contains("Error", StringComparison.OrdinalIgnoreCase)) && prop.Value.ValueKind == JsonValueKind.String)
                {
                    return prop.Value.GetString();
                }
            }

            // 4. Se não encontrar, retorna o JSON completo (limitado)
            return jsonContent.Length > 500 ? jsonContent[..500] + "..." : jsonContent;
        }
        catch
        {
            return jsonContent.Length > 500 ? jsonContent[..500] + "..." : jsonContent;
        }
    }
}
