using System.Security.Claims;
using Application.Interfaces.Services;
using Authentication.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UserAuthInfoResponse GetUserAuthInfo()
    {
        IEnumerable<string> realmRoles = ExtractRealmRoles();
        IEnumerable<string> accountRoles = ExtractAccountRoles();
        IEnumerable<string> organizations = ExtractOrganizations();

        return new UserAuthInfoResponse
        {
            Roles = realmRoles.Concat(accountRoles).Distinct().ToList(),
            Organizations = organizations.ToList(),
            IdUser = GetClaimValue("sub")!,
            Email = GetClaimValue("email")!,
            Name = GetClaimValue("name")!
        };
    }

    public IEnumerable<string> ExtractRealmRoles()
    {
        string? realmAccess = GetClaimValue("realm_access");
        if (string.IsNullOrEmpty(realmAccess))
        {
            return Enumerable.Empty<string>();
        }

        var json = JObject.Parse(realmAccess);
        return json["roles"]?.ToObject<List<string>>() ?? new List<string>();
    }

    public IEnumerable<string> ExtractAccountRoles()
    {
        string? resourceAccess = GetClaimValue("resource_access");
        if (string.IsNullOrEmpty(resourceAccess))
        {
            return Enumerable.Empty<string>();
        }

        var json = JObject.Parse(resourceAccess);
        List<string>? accountRoles = json["account"]?["roles"]?.ToObject<List<string>>();
        return accountRoles ?? new List<string>();
    }

    public IEnumerable<string> ExtractOrganizations()
    {
        string? orgClaim = _httpContextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == "organization")?.Value;

        if (string.IsNullOrEmpty(orgClaim))
        {
            return Enumerable.Empty<string>();
        }

        try
        {
            return JsonConvert.DeserializeObject<List<string>>(orgClaim) ?? new List<string>();
        }
        catch
        {
            return new List<string> { orgClaim };
        }
    }

    public string? GetClaimValue(string claimType)
    {
        IEnumerable<Claim>? claims = _httpContextAccessor.HttpContext?.User?.Claims;

        if (claims == null)
        {
            return null;
        }
        
        string? value = claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        if (!string.IsNullOrEmpty(value))
        {
            return value;
        }
        
        if (claimType == "sub")
        {
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        if (claimType == "email")
        {
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }

        return null;
    }
}
