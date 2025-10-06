using System.Text.Json.Serialization;

namespace Infrastructure.Contracts.IA.Text.Response;

public record TextEnhancerResponse
{

    [JsonPropertyName("text-response")]
    public string? TextResponse { get; set; }

    [JsonPropertyName("usage")]
    public UsageTextEnhancer? Usage { get; set; }

}

public record UsageTextEnhancer
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