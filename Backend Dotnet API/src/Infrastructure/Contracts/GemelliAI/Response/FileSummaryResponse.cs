using System.Text.Json.Serialization;

namespace Infrastructure.Contracts.GemelliAI.Response;

public class FileSummaryResponse
{
    [JsonPropertyName("file_name")]
    public string FileName { get; set; } = string.Empty;

    [JsonPropertyName("resume")]
    public string Resume { get; set; } = string.Empty;
}
