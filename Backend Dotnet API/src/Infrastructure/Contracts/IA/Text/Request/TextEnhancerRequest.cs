using System.Text.Json.Serialization;
using Domain.Enums;

namespace Infrastructure.Contracts.IA.Text.Request;

public class TextEnhancerRequest
{

    [JsonPropertyName("text")]
    public string? Text { get; set; }

}