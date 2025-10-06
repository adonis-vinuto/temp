using System.Security.Claims;
using Application.Interfaces.Repositories;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Extensions;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider, IDataConfigRepository dataConfig)
    {
        ClaimsPrincipal user = context.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            string? organization = user.Claims
                .Where(c => c.Type == "organization")
                .Select(c => c.Value)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(organization))
            {
                string? tenantInfo = await dataConfig.GetTenantConnectionStringAsync(organization);

                if (!string.IsNullOrEmpty(tenantInfo))
                {
                    tenantProvider.SetConnectionString(tenantInfo);
                }
            }
        }

        await _next(context);
    }
}