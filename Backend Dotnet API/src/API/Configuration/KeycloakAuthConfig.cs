using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Text.Json;

namespace API.Configuration;

public static class KeycloakAuthConfig
{
    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["Keycloak:Authority"];
                options.Audience = configuration["Keycloak:Audience"];
                options.RequireHttpsMetadata = false;
                
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                        string? clientId = configuration["Keycloak:ClientId"];
                        
                        // Map client roles
                        string? resourceAccess = context.Principal?.FindFirst("resource_access")?.Value;
                        if (!string.IsNullOrEmpty(resourceAccess) && !string.IsNullOrEmpty(clientId))
                        {
                            var resourceJson = JsonDocument.Parse(resourceAccess);
                            if (resourceJson.RootElement.TryGetProperty(clientId, out JsonElement clientRoles) &&
                                clientRoles.TryGetProperty("roles", out JsonElement clientRolesList))
                            {
                                foreach (JsonElement role in clientRolesList.EnumerateArray())
                                {
                                    claimsIdentity?.AddClaim(new Claim(ClaimTypes.Role, role.GetString() ?? string.Empty));
                                }
                            }
                        }
                        
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}