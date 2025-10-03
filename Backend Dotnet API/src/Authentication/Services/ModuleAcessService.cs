using System.Security.Claims;
using System.Text.Json;
using Authentication.Requirements;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Authentication.Services;

public class ModuleAccessService : AuthorizationHandler<ModuleAccessRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ModuleAccessService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ModuleAccessRequirement requirement)
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        
        if (httpContext?.Request.RouteValues.TryGetValue("module", out object? moduleValue) != true)
        {
            return Task.CompletedTask;
        }

        string? moduleString = moduleValue?.ToString();
        if (string.IsNullOrWhiteSpace(moduleString))
        {
            return Task.CompletedTask;
        }

        if (!Enum.TryParse(typeof(Module), moduleString, ignoreCase: true, out object? moduleEnumObj))
        {
            return Task.CompletedTask;
        }

        string moduleName = moduleEnumObj!.ToString()!;
        
        string? realmAccessClaim = context.User.FindFirst("realm_access")?.Value;
        if (string.IsNullOrWhiteSpace(realmAccessClaim))
        {
            return Task.CompletedTask;
        }
        
        using var doc = JsonDocument.Parse(realmAccessClaim);
        if (doc.RootElement.TryGetProperty("roles", out JsonElement rolesEl) && rolesEl.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement role in rolesEl.EnumerateArray())
            {
                if (role.ValueKind == JsonValueKind.String &&
                    role.GetString()!.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                {
                    context.Succeed(requirement);
                    break;
                }
            }
        }
        
        return Task.CompletedTask;
    }
}