using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Infrastructure.Contracts.GemelliAI.Request;

public class ChatRequest
{
    [JsonPropertyName("id_session")]
    public string? IdSession { get; set; }

    [JsonPropertyName("id_agent")]
    public string IdAgent { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("module")]
    public string Module { get; set; } = string.Empty;

    [JsonPropertyName("organization")]
    public string Organization { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public ChatUser User { get; set; } = new();

    [JsonPropertyName("preferences")]
    public Dictionary<string, string>? Preferences { get; set; }

    [JsonPropertyName("chat_history")]
    public List<ChatHistoryItem> ChatHistory { get; set; } = new();

    [JsonPropertyName("documents")]
    public List<string> Documents { get; set; } = new();

    [JsonPropertyName("agent_type")]
    public string AgentType { get; set; } = string.Empty;
}

public class ChatUser
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}

public class ChatHistoryItem
{
    [JsonPropertyName("role")]
    public int Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}
