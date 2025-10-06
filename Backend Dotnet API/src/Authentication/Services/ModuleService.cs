using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;

namespace Authentication.Services;


public class ModuleService : IModuleService
{
    public bool HasAccessToModule(UserAuthInfoResponse user, Module module)
    {
        if (user == null || user.Roles == null)
        {
            return false;
        }
        
        return user.Roles.Any(role => 
            role.Equals(module.ToString(), StringComparison.OrdinalIgnoreCase));
    }
}