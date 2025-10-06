using Application.AppConfig;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Infrastructure.Common;
using Infrastructure.Context;
using Infrastructure.HttpClient.Handlers;
using Infrastructure.HttpClient.GemelliAI;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using GemelliApi.Infrastructure.Services;
using Refit;

namespace Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMemoryCache();

        services.AddDbContext<GemelliApiContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("GemelliApiDbConnection"),
                ServerVersion.AutoDetect(configuration.GetConnectionString("GemelliApiDbConnection")),
                mySqlOptions => mySqlOptions.EnableStringComparisonTranslations())
            );

        services.AddDbContext<TenantDbContext>((serviceProvider, options) =>
        {
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();

            string? connectionString = tenantProvider.GetConnectionString();

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Tenant connection string not set.");
            }

            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                mySqlOptions => mySqlOptions.EnableStringComparisonTranslations()
            );
        });

        IConfigurationSection GemelliApiSection = configuration.GetSection(nameof(GemelliApiSettings));
        services.Configure<GemelliApiSettings>(GemelliApiSection);

        IConfigurationSection gemelliAISettingsSection = configuration.GetSection(nameof(GemelliAISettings));
        services.Configure<GemelliAISettings>(gemelliAISettingsSection);

        IConfigurationSection templateEmailSettingsSection = configuration.GetSection(nameof(TemplateEmailSettings));
        services.Configure<TemplateEmailSettings>(templateEmailSettingsSection);

        IConfigurationSection smtpSettingsSection = configuration.GetSection(nameof(SmtpSettings));
        services.Configure<SmtpSettings>(smtpSettingsSection);

        services.AddScoped<ITenantInitializer, TenantInitializer>();
        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFileUploadService, FileUploadService>();
        services.AddScoped<IChatSessionRepository, ChatSessionRepository>();
        services.AddScoped<ISeniorService, SeniorService>();

        return services;
    }

    public static IServiceCollection AddRepositoriesServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPaginacao<>), typeof(Paginacao<>));
        services.AddScoped<IDataConfigRepository, DataConfigRepository>();

        // Api
        services.AddScoped<IAgentRepository, AgentRepository>();
        services.AddScoped<ITwilioConfigRepository, TwilioConfigRepository>();
        services.AddScoped<IKnowledgeRepository, KnowledgeRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ISalaryHistoryRepository, SalaryHistoryRepository>();
        services.AddScoped<IPayrollRepository, PayrollRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IChatHistoryRepository, ChatHistoryRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();

        // SeniorHcmConfig
        services.AddScoped<ISeniorHcmConfigRepository, SeniorHcmConfigRepository>();

        // SeniorErpConfig
        services.AddScoped<ISeniorErpConfigRepository, SeniorErpConfigRepository>();

        // Infra
        services.AddScoped<IFileRepository, FileRepository>();

        return services;
    }

    public static IServiceCollection AddRefitServices(this IServiceCollection services)
    {
        services.AddTransient<AuthHeaderHandler>();

        // Cliente GemelliAI - SIMPLES
        services.AddRefitClient<IGemelliAIClient>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                GemelliAISettings settings =
                    serviceProvider.GetRequiredService<IOptions<GemelliAISettings>>().Value;

                if (!string.IsNullOrWhiteSpace(settings.BaseUrl))
                {
                    client.BaseAddress = new Uri(settings.BaseUrl);
                }

                client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);

                if (!client.DefaultRequestHeaders.Contains("Accept"))
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                }
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddScoped<IGemelliAIService, GemelliAIService>();

        return services;
    }
}