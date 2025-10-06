using Authentication.Services;
using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace Authentication.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IModuleService, ModuleService>();
        services.AddSingleton<IAuthorizationHandler, ModuleAccessService>();

        return services;
    }
}
