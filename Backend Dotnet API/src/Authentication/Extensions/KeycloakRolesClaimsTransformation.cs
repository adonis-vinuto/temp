using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace Authentication.Extensions;

public class KeycloakRolesClaimsTransformation : IClaimsTransformation
{
    private readonly IConfiguration _configuration;

    public KeycloakRolesClaimsTransformation(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is not ClaimsIdentity identity)
        {
            return Task.FromResult(principal);
        }
        
        AddRolesFromClaim(principal, identity, "realm_access", "roles");
        
        string clientId = _configuration["Keycloak:Resource"];
        AddClientRolesFromClaim(principal, identity, "resource_access", clientId!);
        
        return Task.FromResult(principal);
    }

    private static void AddRolesFromClaim(ClaimsPrincipal principal, ClaimsIdentity identity, 
        string claimType, string rolesPath)
    {
        Claim? claim = principal.FindFirst(claimType);
        if (claim == null)
        {
            return;
        }
        
        using var jsonDoc = JsonDocument.Parse(claim.Value);
        JsonElement element = jsonDoc.RootElement;

        string[] pathParts = rolesPath.Split('.');
        foreach (string part in pathParts)
        {
            if (!element.TryGetProperty(part, out element))
            {
                return;
            }
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement role in element.EnumerateArray())
            {
                string? roleValue = role.GetString();
                if (!string.IsNullOrWhiteSpace(roleValue) &&
                    !identity.HasClaim(ClaimTypes.Role, roleValue))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                }
            }
        }
        
    }

    private static void AddClientRolesFromClaim(ClaimsPrincipal principal, ClaimsIdentity identity, 
        string claimType, string clientId)
    {
        Claim? claim = principal.FindFirst(claimType);
        if (claim == null)
        {
            return;
        }
        
        using var jsonDoc = JsonDocument.Parse(claim.Value);
        
        if (jsonDoc.RootElement.TryGetProperty(clientId, out JsonElement clientElement) &&
            clientElement.TryGetProperty("roles", out JsonElement rolesElement) &&
            rolesElement.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement role in rolesElement.EnumerateArray())
            {
                string? roleValue = role.GetString();
                if (!string.IsNullOrWhiteSpace(roleValue) &&
                    !identity.HasClaim(ClaimTypes.Role, roleValue))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                }
            }
        }
        
        if (jsonDoc.RootElement.TryGetProperty("account", out JsonElement accountElement) &&
            accountElement.TryGetProperty("roles", out JsonElement accountRolesElement) &&
            accountRolesElement.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement role in accountRolesElement.EnumerateArray())
            {
                string? roleValue = role.GetString();
                if (!string.IsNullOrWhiteSpace(roleValue) &&
                    !identity.HasClaim(ClaimTypes.Role, roleValue))
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                }
            }
        }
        
    }
}