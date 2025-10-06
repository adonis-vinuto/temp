using System.Text.Json.Serialization;

namespace Infrastructure.Contracts.GemelliAI.Response;

public class ChatResponse
{
    [JsonPropertyName("message-response")]
    public string MessageResponse { get; set; } = string.Empty;

    [JsonPropertyName("usage")]
    public Usage Usage { get; set; } = new();
}

public class Usage
{
    [JsonPropertyName("model-name")]
    public string ModelName { get; set; } = string.Empty;

    [JsonPropertyName("input-tokens")]
    public int InputTokens { get; set; }

    [JsonPropertyName("output-tokens")]
    public int OutputTokens { get; set; }

    [JsonPropertyName("total-tokens")]
    public int TotalTokens { get; set; }
}
