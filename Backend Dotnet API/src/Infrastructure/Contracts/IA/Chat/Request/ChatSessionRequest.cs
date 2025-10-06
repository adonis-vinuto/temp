using System.Text.Json.Serialization;
using Domain.Enums;

namespace Infrastructure.Contracts.IA.Chat.Request;

public class ChatSessionRequest
{
    [JsonPropertyName("user")]
    public UserChatSession? User { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("chat-history")]
    public ChatHistoryChatSession ChatHistory {get; set;}
    [JsonPropertyName("organization")]
    public string? Organization { get; set; }
    [JsonPropertyName("module")]
    public Module Module { get; set; }

    [JsonPropertyName("id-agent")]
    public string? IdAgent { get; set; }
    

}

public class UserChatSession
{
    [JsonPropertyName("id-user")]
    public string? IdUser { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class ChatHistoryChatSession
{
    [JsonPropertyName("role")]
    public RoleChat Role { get; set; }
    
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
