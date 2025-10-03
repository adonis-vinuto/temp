using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces.Data;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Context;

public class GemelliApiContext : DbContext, IUnitOfWork
{
    private readonly IConfiguration _configuration = null!;
    private readonly IHostEnvironment _env;
    private readonly IUserContextService _userContextService;

    public GemelliApiContext()
    {
    }

    public GemelliApiContext(
        DbContextOptions<GemelliApiContext> options,
        IConfiguration configuration,
        IHostEnvironment env,
        IUserContextService userContextService)
        : base(options)
    {
        _configuration = configuration;
        _env = env;
        _userContextService = userContextService;
    }

    public DbSet<DataConfig> DataConfigs { get; set; }
    public DbSet<Log> Logs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!_env.IsEnvironment("Test"))
        {
            optionsBuilder.UseMySql(
                _configuration.GetConnectionString("GemelliApiDbConnection"),
                ServerVersion.AutoDetect(_configuration.GetConnectionString("GemelliApiDbConnection")),
                mySqlOptions => mySqlOptions.EnableStringComparisonTranslations()
            );
        }
#if DEBUG
        optionsBuilder.LogTo(x => Console.WriteLine(x)).EnableSensitiveDataLogging();
#endif
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(GemelliApiContext).Assembly,
            t => t.Namespace != null && t.Namespace.Contains("ModelConfig.Master")
        );

        foreach (
            Microsoft.EntityFrameworkCore.Metadata.IMutableForeignKey? relationShip in modelBuilder
                .Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys())
        )
        {
            relationShip.DeleteBehavior = DeleteBehavior.ClientSetNull;
        }

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<string>().AreUnicode(false).HaveColumnType("varchar(400)");
        base.ConfigureConventions(configurationBuilder);
    }

    public async Task Commit()
    {
        foreach (
            EntityEntry? entry in ChangeTracker
                .Entries()
                .Where(x => x.Entity.GetType().GetProperty("Cadastro") != null)
        )
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Property("Cadastro").IsModified = false;
            }
        }

        var entries = new List<(
            object? anterior,
            object? novo,
            TypeProcess? tipoProcesso,
            string nomeEntidade)>();

        foreach (EntityEntry? entry in ChangeTracker.Entries().Where(x => x.Entity is not LogUser))
        {
            if (entry.State == EntityState.Added)
            {
                entries.Add(
                    (null, entry.CurrentValues, TypeProcess.Criacao, entry.Entity.GetType().Name)
                );
            }

            if (entry.State == EntityState.Modified)
            {
                entries.Add(
                    ((await entry.GetDatabaseValuesAsync())?.ToObject(),
                        entry.CurrentValues.ToObject(),
                        TypeProcess.Edicao,
                        entry.Entity.GetType().Name)
                );
            }

            if (entry.State == EntityState.Deleted)
            {
                entries.Add(
                    ((await entry.GetDatabaseValuesAsync())?.ToObject(), null, TypeProcess.Exclusao,
                        entry.Entity.GetType().Name)
                );
            }
        }

        await SaveChangesAsync();

        if (entries.Count > 0)
        {
            Guid? idUser = null;
            LogUser? user = _userContextService.GetUser();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var logs = entries.Select(x => new Log(
                idUser,
                user?.Name,
                user?.Email,
                x.nomeEntidade,
                x.tipoProcesso!.Value,
                x.anterior is null ? string.Empty : JsonSerializer.Serialize(x.anterior, options),
                x.novo switch
                {
                    PropertyValues pv => JsonSerializer.Serialize(pv.ToObject(), options),
                    null => string.Empty,
                    _ => JsonSerializer.Serialize(x.novo, options)
                }
            )).ToList();

            Logs.AddRange(logs);
            await SaveChangesAsync();
        }
    }
}
