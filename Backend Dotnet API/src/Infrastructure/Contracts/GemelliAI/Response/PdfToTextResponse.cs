using System.Text.Json.Serialization;

namespace Infrastructure.Contracts.GemelliAI.Response;

public class PdfToTextResponse
{
    [JsonPropertyName("file-content")]
    public string FileContent { get; set; } = string.Empty;
}
