using System.Text.Json.Serialization;
using Domain.Enums;

namespace Infrastructure.Contracts.IA.Chat.Request;

public class KomsalesChatRequest
{
    [JsonPropertyName("user")]
    public UserKomsalesChat? User { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("chat-history")]
    public ChatHistoryKomsalesChat ChatHistory { get; set; }
}

public class UserKomsalesChat
{
    [JsonPropertyName("id-user")]
    public string? IdUser { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class ChatHistoryKomsalesChat
{
    [JsonPropertyName("role")]
    public RoleChat Role { get; set; }
    
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
