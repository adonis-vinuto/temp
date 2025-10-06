using System.Net;
using System.Text.Json;
using GemelliApi.Application.Common.Responses;
using GemelliApi.Application.Exceptions;

namespace GemelliApi.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadRequestException ex)
        {
            var response = ApiResponse<string>.ErrorResponse(ex.Message, ex.Errors!);
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, response);
        }
        catch (NotFoundException ex)
        {
            var response = new ApiResponse<string>
            {
                Success = false,
                Message = ex.Message
            };
            await WriteResponseAsync(context, HttpStatusCode.NotFound, response);
        }
        catch (Exception)
        {
            var response = new ApiResponse<string>
            {
                Success = false,
                Message = "Erro interno no servidor."
            };
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, response);
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, object response)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
    }
}
