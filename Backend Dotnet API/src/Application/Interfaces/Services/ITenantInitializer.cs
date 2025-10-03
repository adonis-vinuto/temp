using Application.DTOs.DataConfig;

namespace Application.Interfaces.Services;

public interface ITenantInitializer
{
    Task EnsureDatabaseMigrated(string connectionString);
    Task<bool> TestConnectionAsync(string connectionString);
    Task<TestConnectionResponse> TestConnectionWithMessageAsync(string connectionString);
}

