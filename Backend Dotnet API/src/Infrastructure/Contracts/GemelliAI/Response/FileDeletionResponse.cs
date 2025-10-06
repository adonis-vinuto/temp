using System.Text.Json.Serialization;

namespace Infrastructure.Contracts.GemelliAI.Response;

public class FileDeletionResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }

    [JsonPropertyName("file_info")]
    public FileInfoResponse? FileInfo { get; set; }

    public class FileInfoResponse
    {
        [JsonPropertyName("file_name")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("resume")]
        public string? Resume { get; set; }
    }
}
