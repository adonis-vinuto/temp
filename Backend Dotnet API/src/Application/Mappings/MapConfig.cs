using Mapster;
using Domain.Entities;
using Application.DTOs.DataConfig;
using Application.DTOs.Knowledge;
using Application.DTOs.SeniorErpConfig;
using Application.DTOs.SeniorHcmConfig;
using Application.DTOs.Agent;
using Application.DTOs.ChatHistory;
using System.Globalization;
using DomainFile = Domain.Entities.File;

namespace GemelliApi.Application.Mappings;

public static class MapConfig
{
    public static void Register(TypeAdapterConfig config)
    {
        // DataConfig
        config.NewConfig<DataConfig, DataConfigResponse>();
        
        // SeniorErpConfig
        config.NewConfig<SeniorErpConfig, SeniorErpConfigResponse>()
            .Map(dest => dest.IdSeniorErpConfig, src => src.Id);
        
        // SeniorHCMConfig
        config.NewConfig<SeniorHcmConfig, SeniorHcmConfigResponse>()
            .Map(dest => dest.IdSeniorHcmConfig, src => src.Id);
        
        // Knowledge
        config.NewConfig<Knowledge, KnowledgeResponse>()
            .Map(dest => dest.IdKnowledge, src => src.Id);

        // FullAgentResponse
        config.NewConfig<Agent, AgentByIdResponse>()
            .Map(dest => dest.TypeAgent, src => src.Type)
            .Map(dest => dest.Files, src => src.Files)
            .Map(dest => dest.ChatSessions, src => src.Chats)
            .Map(dest => dest.ChatHistory, src => src.Chats.SelectMany(c => c.ChatHistory))
            .Map(dest => dest.TwilioConfig, src => (AgentTwilioConfig)null)
            .Map(dest => dest.SeniorHcmConfig, src => src.SeniorHcmConfigs)
            .Map(dest => dest.Knowledge, src => src.Knowledges)
            .Map(dest => dest.SeniorErpConfig, src => src.SeniorErpConfigs);

        // AgentFiles
        config.NewConfig<DomainFile, AgentFiles>()
            .Map(dest => dest.UrlFile, src => src.FileName);

        // AgentChatSessions
        config.NewConfig<ChatSession, AgentChatSessions>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.ExternalSessionId, src => src.IdSession)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.MessageResponse, src => src.ChatHistory.LastOrDefault() != null ? src.ChatHistory.LastOrDefault()!.Content : null);

        // AgentChatHistory
        config.NewConfig<ChatHistory, AgentChatHistory>()
            .Map(dest => dest.SendDate, src => src.CreatedAt.ToString(CultureInfo.InvariantCulture))
            .Map(dest => dest.Usage, src => new ChatHistoryUsageDto
            {
                TotalTokens = src.TotalTokens,
                TotalTime = (int)src.TotalTime
            });

        // ChatHistoryResponse
        config.NewConfig<ChatHistory, ChatHistoryResponse>()
            .Map(dest => dest.SendDate, src => src.CreatedAt.ToString(CultureInfo.InvariantCulture))
            .Map(dest => dest.Usage, src => new UsageDto
            {
                TotalTokens = src.TotalTokens,
                TotalTime = (int)src.TotalTime
            });

        // AgentSeniorHcmConfig
        config.NewConfig<SeniorHcmConfig, AgentSeniorHcmConfig>()
            .Map(dest => dest.IdSeniorHcmConfig, src => src.Id);

        // AgentKnowledge
        config.NewConfig<Knowledge, AgentKnowledge>()
            .Map(dest => dest.IdKnowledge, src => src.Id);

        // AgentSeniorErpConfig
        config.NewConfig<SeniorErpConfig, AgentSeniorErpConfig>()
            .Map(dest => dest.IdSeniorErpConfig, src => src.Id);

        // AgentTwilioConfig
        config.NewConfig<TwilioConfig, AgentTwilioConfig>()
            .Map(dest => dest.AgentName, src => src.Agent != null ? src.Agent.Name : string.Empty);
    }
}
