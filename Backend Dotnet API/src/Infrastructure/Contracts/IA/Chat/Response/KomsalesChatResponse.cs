using System.Text.Json.Serialization;

namespace Infrastructure.IA.Chat.Response;

public record KomsalesChatResponse
{
    [JsonPropertyName("message-response")]
    public string? MessageResponse { get; set; }

    [JsonPropertyName("usage")]
    public UsageKomsalesChat? Usage { get; set; }

}

public record UsageKomsalesChat
{
    [JsonPropertyName("model-name")]
    public string? ModelName { get; set; }

    [JsonPropertyName("finish-reason")]
    public string? FinishReason { get; set; }

    [JsonPropertyName("input-tokens")]
    public int? InputTokens { get; set; }

    [JsonPropertyName("output-tokens")]
    public int? OutputTokens { get; set; }

    [JsonPropertyName("total-tokens")]
    public int? TotalTokens { get; set; }
}