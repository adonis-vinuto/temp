using Application.Interfaces.Services;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Application.DTOs.DataConfig;

namespace Infrastructure.Services;

public class TenantInitializer : ITenantInitializer
{
    public async Task EnsureDatabaseMigrated(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString),
            mySqlOptions => mySqlOptions.EnableStringComparisonTranslations()
        );

        await using var db = new TenantDbContext(optionsBuilder.Options);
        await db.Database.MigrateAsync();
    }

    public async Task<bool> TestConnectionAsync(string connectionString)
    {
        try
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                mySqlOptions => mySqlOptions.EnableStringComparisonTranslations()
            );

            await using var db = new TenantDbContext(optionsBuilder.Options);
            return await db.Database.CanConnectAsync();
        }
        catch
        {
            return false;
        }
    }

    public async Task<TestConnectionResponse> TestConnectionWithMessageAsync(string connectionString)
    {
        var response = new TestConnectionResponse();

        try
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                mySqlOptions => mySqlOptions.EnableStringComparisonTranslations()
            );

            await using var db = new TenantDbContext(optionsBuilder.Options);

            if (await db.Database.CanConnectAsync())
            {
                response.Success = true;
                response.Message = "Conexão bem-sucedida.";
            }
            
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Erro ao testar a conexão: {ex.Message}";
        }

        return response;
    }

}

