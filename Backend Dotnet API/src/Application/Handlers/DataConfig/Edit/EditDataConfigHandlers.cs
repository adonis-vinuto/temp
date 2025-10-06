using Application.DTOs.DataConfig;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.DataConfig.Edit;

public class EditDataConfigHandler : BaseHandler
{
    private readonly IDataConfigRepository _dataConfigRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly ITenantInitializer _tenantInitializer;

    public EditDataConfigHandler(
        IDataConfigRepository dataConfigRepository,
        IAuthenticationService authenticationService,
        ITenantInitializer tenantInitializer)
    {
        _dataConfigRepository = dataConfigRepository;
        _authenticationService = authenticationService;
        _tenantInitializer = tenantInitializer;
    }

    public async Task<ErrorOr<DataConfigResponse>> Handle(EditDataConfigRequest request, CancellationToken cancellationToken)
    {
        if (Validate(request, new EditDataConfigRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }
        
        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }
        
        Domain.Entities.DataConfig? dataConfig = await _dataConfigRepository.SearchByIdAsync(request.Id, user.Organizations.FirstOrDefault()!, cancellationToken);

        if (dataConfig is null)
        {
            return DataConfigErrors.DataConfigNotFound;
        }

        dataConfig.SqlHost = request.SqlHost ?? dataConfig.SqlHost;
        dataConfig.SqlPort = request.SqlPort ?? dataConfig.SqlPort;
        dataConfig.SqlUser = request.SqlUser ?? dataConfig.SqlUser;
        dataConfig.SqlPassword = request.SqlPassword ?? dataConfig.SqlPassword;
        dataConfig.SqlDatabase = request.SqlDatabase ?? dataConfig.SqlDatabase;
        dataConfig.BlobConnectionString = request.BlobConnectionString ?? dataConfig.BlobConnectionString;
        dataConfig.BlobContainerName = request.BlobContainerName ?? dataConfig.BlobContainerName;
        
        string tenantConnectionString = $"Host={dataConfig.SqlHost};Port={dataConfig.SqlPort};Database={dataConfig.SqlDatabase};Username={dataConfig.SqlUser};Password={dataConfig.SqlPassword};";

        if (!await _tenantInitializer.TestConnectionAsync(tenantConnectionString))
        {
            return DataConfigErrors.DatabaseConnectionFail;
        }

        await _tenantInitializer.EnsureDatabaseMigrated(tenantConnectionString);

        _dataConfigRepository.Edit(dataConfig);
        await _dataConfigRepository.UnitOfWork.Commit();
        
        DataConfigResponse dataConfigResponse = dataConfig.Adapt<DataConfigResponse>();
        return dataConfigResponse;
    }
}
