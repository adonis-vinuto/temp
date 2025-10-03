using Application.DTOs.DataConfig;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Errors;
using ErrorOr;

namespace Application.Handlers.DataConfig.TestConfig;

public class TestDataConfigHandler : BaseHandler
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ITenantInitializer _tenantInitializer;

    public TestDataConfigHandler(
        IAuthenticationService authenticationService,
        ITenantInitializer tenantInitializer)
    {
        _authenticationService = authenticationService;
        _tenantInitializer = tenantInitializer;
    }

    public async Task<ErrorOr<TestConnectionResponse>> Handle(TestDataConfigRequest request)
    {
        if (Validate(request, new TestDataConfigRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }

        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        string tenantConnectionString = $"Host={request.Host};Port={request.Port};Database={request.Database};Username={request.User};Password={request.Password};";

        TestConnectionResponse testConnectionResponse = await _tenantInitializer.TestConnectionWithMessageAsync(tenantConnectionString);
        if (!testConnectionResponse.Success)
        {
            return testConnectionResponse;
        }

        return testConnectionResponse;
    }
}