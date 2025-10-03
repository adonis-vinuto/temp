using System.Security.Claims;

namespace Authentication.Extensions;

public static class ClaimsPrincipalExtensions
{
  public static string? GetUserId(this ClaimsPrincipal user)
  {
    return user.FindFirst("user_id")?.Value;
  }

  public static string? GetUserEmail(this ClaimsPrincipal user)
  {
    return user.FindFirst("user_email")?.Value;
  }

  public static string? GetUserName(this ClaimsPrincipal user)
  {
    return user.FindFirst("user_name")?.Value;
  }
  
  public static string? GetUserOrganization(this ClaimsPrincipal user)
  {
    return user.FindFirst("organization")?.Value;
  }

  public static List<string> GetUserRoles(this ClaimsPrincipal user)
  {
    return user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
  }
}
