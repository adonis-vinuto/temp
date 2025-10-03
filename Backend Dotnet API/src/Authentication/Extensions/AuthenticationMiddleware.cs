using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Authentication.Extensions;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserContextService userContextService)
    {
        string? userId = context.User.GetUserId();
        string? email = context.User.GetUserEmail();
        string? name = context.User.GetUserName();

        if (Guid.TryParse(userId, out Guid id) is var status && status)
        {
            userContextService.SetUser(new LogUser
            {
                Id = id,
                Name = name,
                Email = email
            });
        }

        await _next(context);
    }

}