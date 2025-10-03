using System.Text.Json.Serialization;
using Domain.Enums;

namespace Infrastructure.Contracts.IA.Chat.Request;

public class ChatTwillioRequest
{
    [JsonPropertyName("user")]
    public UserChatTwillio? User { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("chat-history")]
    public ChatHistoryChatTwillio ChatHistory { get; set; }

    [JsonPropertyName("organization")]
    public string? Organization { get; set; }

    [JsonPropertyName("module")]
    public Module Module { get; set; }

    [JsonPropertyName("id-agent")]
    public string? IdAgent { get; set; }
}

public class UserChatTwillio
{
    [JsonPropertyName("id-employee")]
    public string? IdEmployee { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class ChatHistoryChatTwillio
{
    [JsonPropertyName("role")]
    public RoleChat Role { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }
}