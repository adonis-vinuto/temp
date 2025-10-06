using Application.DTOs.DataConfig;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Authentication.Models;
using Domain.Enums;
using Domain.Errors;
using ErrorOr;
using Mapster;

namespace Application.Handlers.DataConfig.Create;

public class CreateDataConfigHandler : BaseHandler
{
    private readonly IDataConfigRepository _dataConfigRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly ITenantInitializer _tenantInitializer;

    public CreateDataConfigHandler(IDataConfigRepository dataConfigRepository, 
        IAuthenticationService authenticationService,
        ITenantInitializer tenantInitializer)
    {
        _dataConfigRepository = dataConfigRepository;
        _authenticationService = authenticationService;
        _tenantInitializer = tenantInitializer;
    }

    public async Task<ErrorOr<DataConfigResponse>> Handle(CreateDataConfigRequest request, CancellationToken cancellationToken)
    {
        if (Validate(request, new CreateDataConfigRequestValidator()) is var resultado && resultado.Any())
        {
            return resultado;
        }
        
        UserAuthInfoResponse user = _authenticationService.GetUserAuthInfo();

        if (user is null)
        {
            return UserErrors.UserNotFound;
        }

        Domain.Entities.DataConfig? organization = await _dataConfigRepository.GetDataConfigByOrganization(user.Organizations.FirstOrDefault()!, cancellationToken);
        
        if (organization is not null)
        {
            return DataConfigErrors.DataConfigAlreadyRegistered;
        }
        
        string? blobConnection = request.BlobConnectionString;
        string? blobName = request.BlobContainerName;
        
        // Corrigir e adicionar opção de criar banco da statum se cliente não quiser usar o proprio
        // Corrigir o blob connection padrão da statum
        // Corrigir ou remover modulo da criação do data config.
        if (blobConnection is null || blobName is null)
        {
            blobConnection = "BlobConnection";
            blobName = "BlobName";
        }
        
        var dataConfig = new Domain.Entities.DataConfig(
            Module.People,
            user.Organizations.FirstOrDefault()!,
            request.SqlHost,
            request.SqlPort,
            request.SqlUser,
            request.SqlPassword,
            request.SqlDatabase,
            blobConnection,
            blobName
        );
        
        string tenantConnectionString = $"Host={dataConfig.SqlHost};Port={dataConfig.SqlPort};Database={dataConfig.SqlDatabase};Username={dataConfig.SqlUser};Password={dataConfig.SqlPassword};";

        if (!await _tenantInitializer.TestConnectionAsync(tenantConnectionString))
        {
            return DataConfigErrors.DatabaseConnectionFail;
        }

        await _tenantInitializer.EnsureDatabaseMigrated(tenantConnectionString);
        
        await _dataConfigRepository.AddAsync(dataConfig, cancellationToken);
        await _dataConfigRepository.UnitOfWork.Commit();
        
        DataConfigResponse dataConfigResponse = dataConfig.Adapt<DataConfigResponse>();
        return dataConfigResponse;
    }
}