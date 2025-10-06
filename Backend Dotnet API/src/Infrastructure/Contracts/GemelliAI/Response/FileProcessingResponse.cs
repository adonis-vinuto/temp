using System.Text.Json.Serialization;

namespace Infrastructure.Contracts.GemelliAI.Response;

public class FileProcessingResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("file_info")]
    public FileInfoResponse FileInfo { get; set; } = new();

    [JsonPropertyName("qdrant_info")]
    public QdrantInfoResponse? QdrantInfo { get; set; }

    public class FileInfoResponse
    {
        [JsonPropertyName("file_name")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("resume")]
        public string Resume { get; set; } = string.Empty;
    }

    public class QdrantInfoResponse
    {
        [JsonPropertyName("collection_name")]
        public string CollectionName { get; set; } = string.Empty;

        [JsonPropertyName("documents_inserted")]
        public int DocumentsInserted { get; set; }

        [JsonPropertyName("insertion_result")]
        public object? InsertionResult { get; set; }
    }
}
