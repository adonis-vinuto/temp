using System.Text.Json.Serialization;

namespace Infrastructure.Contracts.GemelliAI.Request;

public class DocumentSummaryRequest
{
    [JsonPropertyName("file-content")]
    public string FileContent { get; set; } = string.Empty;
}
