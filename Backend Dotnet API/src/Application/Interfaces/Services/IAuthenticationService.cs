using Authentication.Models;

namespace Application.Interfaces.Services;

public interface IAuthenticationService
{
    UserAuthInfoResponse GetUserAuthInfo();
    IEnumerable<string> ExtractRealmRoles();
    IEnumerable<string> ExtractOrganizations();
}