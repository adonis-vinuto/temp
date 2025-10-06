using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces.Data;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using File = Domain.Entities.File;

namespace Infrastructure.Context;

public class TenantDbContext : DbContext, IUnitOfWork
{
    private readonly IUserContextService? _userContextService;
    private readonly ITenantProvider? _tenantProvider;

    public TenantDbContext(DbContextOptions<TenantDbContext> options)
        : base(options)
    {
    }

    public TenantDbContext(
        DbContextOptions<TenantDbContext> options,
        IUserContextService userContextService,
        ITenantProvider tenantProvider)
        : base(options)
    {
        _userContextService = userContextService;
        _tenantProvider = tenantProvider;
    }

    public DbSet<Agent> Agents { get; set; }
    public DbSet<ChatSession> Chats { get; set; }
    public DbSet<ChatHistory> ChatsHistory { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<Knowledge> Knowledges { get; set; }
    public DbSet<Payroll> Payrolls { get; set; }
    public DbSet<SalaryHistory> SalaryHistories { get; set; }
    public DbSet<SeniorErpConfig> SeniorErpConfigs { get; set; }
    public DbSet<SeniorHcmConfig> SeniorHcmConfigs { get; set; }
    public DbSet<TwilioConfig> TwilioConfigs { get; set; }
    public DbSet<Log> Logs { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && _tenantProvider != null)
        {
            string? connectionString = _tenantProvider.GetConnectionString();

            if (!string.IsNullOrEmpty(connectionString))
            {

            optionsBuilder.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                mySqlOptions => mySqlOptions.EnableStringComparisonTranslations()
            );

            }
            else
            {
                throw new InvalidOperationException("Connection string do tenant não foi configurada.");
            }
        }

#if DEBUG
        optionsBuilder.LogTo(x => Console.WriteLine(x)).EnableSensitiveDataLogging();
#endif
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(TenantDbContext).Assembly,
            t => t.Namespace != null && t.Namespace.Contains("ModelConfig.Tenant")
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
            LogUser? user = _userContextService?.GetUser();

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
