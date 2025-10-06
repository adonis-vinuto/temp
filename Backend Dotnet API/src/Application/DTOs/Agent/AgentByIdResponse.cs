using Domain.Enums;

namespace Application.DTOs.Agent;

public class AgentByIdResponse
{
    public Guid Id { get; set; }
    public string Organization { get; set; }
    public Module Module { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid IdUser { get; set; }
    public TypeAgent TypeAgent { get; set; }
    public IEnumerable<AgentFiles>? Files { get; set; }
    public IEnumerable<AgentChatSessions>? ChatSessions { get; set; }
    public IEnumerable<AgentChatHistory>? ChatHistory { get; set; }
    public AgentTwilioConfig? TwilioConfig { get; set; }
    public IEnumerable<AgentSeniorHcmConfig>? SeniorHcmConfig { get; set; }
    public IEnumerable<AgentKnowledge>? Knowledge { get; set; }
    public IEnumerable<AgentSeniorErpConfig>? SeniorErpConfig { get; set; }
}

public class AgentFiles
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string UrlFile { get; set; }
    public IEnumerable<FilePageResponse>? Pages { get; set; }
}

public class FilePageResponse
{
    public Guid Id { get; set; }
    public int PageNumber { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? ResumePage { get; set; }
}

public class AgentChatSessions
{
    public Guid Id { get; set; }
    public string? MessageResponse { get; set; }
    public string? ExternalSessionId { get; set; }
    public string? Title { get; set; }
}

public class AgentChatHistory
{
    public RoleChat? Role { get; set; }
    public string? Content { get; set; }
    public string? SendDate { get; set; }
    public ChatHistoryUsageDto? Usage { get; set; }
}

public class ChatHistoryUsageDto
{
    public int TotalTokens { get; set; }
    public int TotalTime { get; set; }
}

public class AgentTwilioConfig
{
    public Guid Id { get; set; }
    public Guid IdAgent { get; set; }
    public string AgentName { get; set; } = string.Empty;
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string WebhookUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class AgentSeniorHcmConfig
{
    public Guid IdSeniorHcmConfig { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? WsdlUrl { get; set; }
}

public class AgentKnowledge
{
    public Guid IdKnowledge { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Origin? Origin { get; set; }

}

public class AgentSeniorErpConfig
{
    public Guid IdSeniorErpConfig { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? WsdlUrl { get; set; }
}

