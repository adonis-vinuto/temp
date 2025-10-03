using System.Text.Json.Serialization;

namespace Infrastructure.Contracts.IA.Pdf.Response;

public record DocSortResponse
{
    [JsonPropertyName("file-name")]
    public string? FileName { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }

    [JsonPropertyName("usage")]
    public UsageDocSort? Usage { get; set; }
}

public record UsageDocSort
{
    [JsonPropertyName("completion-tokens")]
    public int? CompletionTokens { get; set; }

    [JsonPropertyName("prompt-tokens")]
    public int? PromptTokens { get; set; }

    [JsonPropertyName("total-tokens")]
    public int? TotalTokens { get; set; }

    [JsonPropertyName("completion-time")]
    public int? CompletionTime { get; set; }

    [JsonPropertyName("prompt-time")]
    public int? PromptTime { get; set; }

    [JsonPropertyName("queue-time")]
    public int? QueueTime { get; set; }

    [JsonPropertyName("total-time")]
    public int? TotalTime { get; set; }
}