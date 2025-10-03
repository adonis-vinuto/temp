using Authentication.Models;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface IModuleService
{
    bool HasAccessToModule(UserAuthInfoResponse user, Module module);
}