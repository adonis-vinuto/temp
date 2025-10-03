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
    public Usage Usage { get; set; } = new();
}
