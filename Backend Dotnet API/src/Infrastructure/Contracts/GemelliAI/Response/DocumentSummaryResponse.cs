using System.Text.Json.Serialization;

namespace Infrastructure.Contracts.GemelliAI.Response;

public class DocumentSummaryResponse
{
    [JsonPropertyName("name-file")]
    public string NameFile { get; set; } = string.Empty;

    [JsonPropertyName("description-file")]
    public string DescriptionFile { get; set; } = string.Empty;

    [JsonPropertyName("resume-file")]
    public string ResumeFile { get; set; } = string.Empty;

    [JsonPropertyName("usage")]
    public DocumentSummaryUsage Usage { get; set; } = new();
}

public class DocumentSummaryUsage
{
    [JsonPropertyName("input_tokens")]
    public int InputTokens { get; set; }

    [JsonPropertyName("output_tokens")]
    public int OutputTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}
