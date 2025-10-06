using Application.Handlers.Agent.Create;
using Application.Handlers.Agent.Edit;
using Application.Handlers.Agent.Remove;
using Application.Handlers.Agent.Search;
using Application.Handlers.Agent.SearchById;
using Application.Handlers.Ai.Agent.SearchByIdAndOrganization;
using Application.Handlers.Chat.FirstMessage;
using Application.Handlers.Chat.SendMessage;
using Application.Handlers.ChatHistory.SearchByIdAgent;
using Application.Handlers.Dashboard.GetDashboard;
using Application.Handlers.DataConfig.Create;
using Application.Handlers.DataConfig.Edit;
using Application.Handlers.DataConfig.Search;
using Application.Handlers.DataConfig.SearchById;
using Application.Handlers.DataConfig.TestConfig;
using Application.Handlers.File.AttachToAgent;
using Application.Handlers.File.DetachFromAgent;
using Application.Handlers.File.Create;
using Application.Handlers.File.Remove;
using Application.Handlers.File.Search;
using Application.Handlers.File.SearchById;
using Application.Handlers.Knowledge.AttachToAgent;
using Application.Handlers.Knowledge.Create;
using Application.Handlers.Knowledge.CreateImportSenior;
using Application.Handlers.Knowledge.Edit;
using Application.Handlers.Knowledge.ImportExcel;
using Application.Handlers.Knowledge.Remove;
using Application.Handlers.Knowledge.Search;
using Application.Handlers.Knowledge.SearchById;
using Application.Handlers.People.Employee.Create;
using Application.Handlers.People.Employee.Edit;
using Application.Handlers.People.Employee.Remove;
using Application.Handlers.People.Employee.Search;
using Application.Handlers.People.Employee.SearchById;
using Application.Handlers.People.Payroll.Create;
using Application.Handlers.People.Payroll.Edit;
using Application.Handlers.People.Payroll.Remove;
using Application.Handlers.People.Payroll.Search;
using Application.Handlers.People.Payroll.SearchById;
using Application.Handlers.People.SalaryHistory.Create;
using Application.Handlers.People.SalaryHistory.Edit;
using Application.Handlers.People.SalaryHistory.Remove;
using Application.Handlers.People.SalaryHistory.SearchByIdAndIdKnowledge;
using Application.Handlers.People.SalaryHistory.SearchByIdKnowledge;
using Application.Handlers.SeniorErpConfig.AttachToAgent;
using Application.Handlers.SeniorErpConfig.Create;
using Application.Handlers.SeniorErpConfig.Edit;
using Application.Handlers.SeniorErpConfig.Remove;
using Application.Handlers.SeniorErpConfig.Search;
using Application.Handlers.SeniorErpConfig.SearchById;
using Application.Handlers.SeniorHcmConfig.AttachToAgent;
using Application.Handlers.SeniorHcmConfig.Create;
using Application.Handlers.SeniorHcmConfig.Edit;
using Application.Handlers.SeniorHcmConfig.Remove;
using Application.Handlers.SeniorHcmConfig.Search;
using Application.Handlers.SeniorHcmConfig.SearchById;
using Application.Handlers.Session.SearchByIdAgent;
using Application.Handlers.Session.SearchTwilioByIdAgent;
using Application.Handlers.TwilioConfig.Create;
using Application.Handlers.TwilioConfig.Edit;
using Application.Handlers.TwilioConfig.Remove;
using Application.Handlers.TwilioConfig.Search;
using Application.Handlers.TwilioConfig.SearchById;
using Application.Interfaces.Services;
using Application.Services;
using GemelliApi.Application.Mappings;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Handlers

        // Agents
        services.AddScoped<CreateAgentHandler>();
        services.AddScoped<SearchAgentByIdHandler>();
        services.AddScoped<SearchAgentsHandler>();
        services.AddScoped<RemoveAgentHandler>();
        services.AddScoped<EditAgentHandler>();

        // DataConfig
        services.AddScoped<CreateDataConfigHandler>();
        services.AddScoped<SearchDataConfigByIdHandler>();
        services.AddScoped<SearchDataConfigsHandler>();
        services.AddScoped<EditDataConfigHandler>();
        services.AddScoped<TestDataConfigHandler>();

        // TwilioConfig
        services.AddScoped<CreateTwilioConfigHandler>();
        services.AddScoped<SearchTwilioConfigByIdHandler>();
        services.AddScoped<SearchTwilioConfigsHandler>();
        services.AddScoped<EditTwilioConfigHandler>();
        services.AddScoped<RemoveTwilioConfigHandler>();

        // Chat
        services.AddScoped<FirstMessageHandler>();
        services.AddScoped<SendChatMessageHandler>();

        // Knowledge
        services.AddScoped<CreateKnowledgeHandler>();
        services.AddScoped<SearchKnowledgeByIdHandler>();
        services.AddScoped<SearchKnowledgesHandler>();
        services.AddScoped<EditKnowledgeHandler>();
        services.AddScoped<AttachToAgentKnowledgeHandler>();
        services.AddScoped<RemoveKnowledgeHandler>();

        // Import Senior
        services.AddScoped<CreateImportSeniorHandler>();

        // Import Excel
        services.AddScoped<ImportExcelKnowledgeHandler>();

        // Employee
        services.AddScoped<CreateEmployeeHandler>();
        services.AddScoped<EditEmployeeHandler>();
        services.AddScoped<RemoveEmployeeHandler>();
        services.AddScoped<SearchEmployeeHandler>();
        services.AddScoped<SearchEmployeeByIdHandler>();

        // SalaryHistory
        services.AddScoped<CreateSalaryHistoryHandler>();
        services.AddScoped<SearchSalaryHistoryIdAndIdKnowledgeHandler>();
        services.AddScoped<SearchSalaryHistoryIdKnowledgeHandler>();
        services.AddScoped<EditSalaryHistoryHandler>();
        services.AddScoped<RemoveSalaryHistoryHandler>();

        // Payroll
        services.AddScoped<CreatePayrollHandler>();
        services.AddScoped<SearchPayrollHandler>();
        services.AddScoped<SearchByIdPayrollHandler>();
        services.AddScoped<EditPayrollHandler>();
        services.AddScoped<RemovePayrollHandler>();

        // SeniorHcmConfig
        services.AddScoped<CreateSeniorHcmConfigHandler>();
        services.AddScoped<EditSeniorHcmConfigHandler>();
        services.AddScoped<SearchSeniorHcmConfigHandler>();
        services.AddScoped<SearchByIdSeniorHcmConfigHandler>();
        services.AddScoped<RemoveSeniorHcmConfigHandler>();
        services.AddScoped<AttachToAgentSeniorHcmConfigHandler>();

        // SeniorErpConfig
        services.AddScoped<CreateSeniorErpConfigHandler>();
        services.AddScoped<EditSeniorErpConfigHandler>();
        services.AddScoped<SearchSeniorErpConfigHandler>();
        services.AddScoped<SearchByIdSeniorErpConfigHandler>();
        services.AddScoped<RemoveSeniorErpConfigHandler>();
        services.AddScoped<AttachToAgentSeniorErpConfigHandler>();

        // Files
        services.AddScoped<CreateFileHandler>();
        services.AddScoped<SearchFileByIdHandler>();
        services.AddScoped<SearchFilesHandler>();
        services.AddScoped<AttachToAgentFileHandler>();
        services.AddScoped<RemoveFileHandler>();
        services.AddScoped<DetachFromAgentFileHandler>();

        // ChatHistory
        services.AddScoped<SearchChatHistoryByIdAgentHandler>();

        // ChatSession
        services.AddScoped<SearchSessionByIdAgentHandler>();
        services.AddScoped<SearchSessionTwilioByIdAgentHandler>();

        //Dashboard
        services.AddScoped<GetDashboardHandler>();

        // Ai
        services.AddScoped<SearchByIdAndOrganizationHandler>();

        // Services
        services.AddScoped<IUserContextService, UserContextService>();
        
        return services;
    }

    public static IServiceCollection AddMapperServices(this IServiceCollection services)
    {
        TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
        config.RegisterMappings();
        config.Default.IgnoreNullValues(true); // Para ignorar valores nulos do request ao editar

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }

    public static void RegisterMappings(this TypeAdapterConfig config)
    {
        MapConfig.Register(config);
    }
}


