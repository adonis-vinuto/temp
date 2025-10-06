using API.Common.Errors;
using API.Models.Config;
using Application.Extensions;
using Authentication.Context;
using Infrastructure.Context;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using GemelliApi.API.Middlewares;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Authentication.Extensions;
using Authentication.Requirements;
using AuthenticationMiddleware = Authentication.Extensions.AuthenticationMiddleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder
    .Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

builder.Services.AddCors(options => options.AddPolicy(
        name: CorsConfig.Nome,
        builder =>
        {
            builder.SetIsOriginAllowed(_ => true);
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
            builder.AllowCredentials();
        }
    ));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration, options =>
{
    options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
    options.TokenValidationParameters.NameClaimType = ClaimTypes.NameIdentifier;
});
builder.Services.AddAuthorization(
    options => options.AddPolicy("ModuleAccessPolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.Requirements.Add(new ModuleAccessRequirement());
        }));
builder.Services.AddTransient<IClaimsTransformation, KeycloakRolesClaimsTransformation>();

builder
    .Services
    .AddApplicationServices()
    .AddMapperServices()
    .AddAuthenticationServices()
    .AddRepositoriesServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddRefitServices();

WebApplication app = builder.Build();

if (!app.Environment.IsEnvironment("Test"))
{
    IServiceScope serviceScope = app.Services.CreateScope();

    AuthContext? authContext = serviceScope.ServiceProvider.GetService<AuthContext>();
    if (authContext != null)
    {
        await authContext.Database.MigrateAsync();
    }

    GemelliApiContext? GemelliApiContext = serviceScope.ServiceProvider.GetService<GemelliApiContext>();
    if (GemelliApiContext != null)
    {
        await GemelliApiContext.Database.MigrateAsync();
    }

    serviceScope.Dispose();
}

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<GlobalErrorHandlerMiddleware>();
app.UseExceptionHandler("/erros");
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantMiddleware>();
app.MapControllers();
await app.RunAsync();

public partial class Program { }
