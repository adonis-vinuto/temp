using Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Services;

public class RoleVerifier : IRoleVerifier
{
    private readonly UserManager<IdentityUser> _userManager;

    public RoleVerifier(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public bool IsUserInCoordenadorRole(string idIdentityUsuario)
    {
        IdentityUser? usuario = _userManager.FindByIdAsync(idIdentityUsuario).Result;
        if (usuario == null)
        {
            return false;
        }

        IList<string> roles = _userManager.GetRolesAsync(usuario).Result;
        return roles.Contains("Coordenador");
    }
}
