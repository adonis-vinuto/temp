using System.Text.Json.Serialization;

namespace Infrastructure.Contracts.GemelliAI.Response;

public class BaseErrorResponse
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("detail")]
    public string Detail { get; set; } = string.Empty;
}

public class ValidationErrorResponse : BaseErrorResponse
{
    [JsonPropertyName("errors")]
    public Dictionary<string, List<string>> Errors { get; set; } = new();
}