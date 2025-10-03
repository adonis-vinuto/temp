using System.Text.Json.Serialization;

namespace Infrastructure.Contracts.IA.Pdf.Response;

public record AnalyzeResponse
{
    [JsonPropertyName("file-name")]
    public string? FileName { get; set; }

    [JsonPropertyName("total-pages")]
    public int? TotalPages { get; set; }

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("answer")]
    public string Answer { get; set; }

    [JsonPropertyName("pages")]
    public IEnumerable<PagesAnalyze> Pages { get; set; }

    [JsonPropertyName("usage")]
    public UsageAnalyze? Usage { get; set; }
}

public record PagesAnalyze
{
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("raw-text")]
    public string? RawText { get; set; }

    [JsonPropertyName("char-count")]
    public int? CharCount { get; set; }

    [JsonPropertyName("error")]
    public string Error { get; set; }
}

public record UsageAnalyze
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