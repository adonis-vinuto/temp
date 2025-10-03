using System.Text.Json.Serialization;

namespace Infrastructure.IA.Chat.Response;

public record ChatTwillioResponse
{
    [JsonPropertyName("message-response")]
    public string? MessageResponse { get; set; }

    [JsonPropertyName("usage")]
    public UsageChatTwillio? Usage { get; set; }

}

public record UsageChatTwillio
{
    [JsonPropertyName("queue-time")]
    public int? QueueTime { get; set; }

    [JsonPropertyName("prompt-tokens")]
    public int? PromptTokens { get; set; }

    [JsonPropertyName("prompt-time")]
    public int? PromptTime { get; set; }

    [JsonPropertyName("completion-tokens")]
    public int? CompletionTokens { get; set; }

    [JsonPropertyName("completion-time")]
    public int? CompletionTime { get; set; }

    [JsonPropertyName("total-tokens")]
    public int? TotalTokens { get; set; }

    [JsonPropertyName("total-time")]
    public int? TotalTime { get; set; }
}