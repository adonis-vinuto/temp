using Domain.Enums;
using System.Text.Json.Serialization;

namespace Infrastructure.Contracts.GemelliAI.Request;

public class FileRequest
{
    [JsonPropertyName("organization")]
    public string Organization { get; set; } = string.Empty;

    [JsonPropertyName("id_agent")]
    public string IdAgent { get; set; } = string.Empty;

}


