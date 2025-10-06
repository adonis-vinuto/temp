using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Contracts.IA.Pdf.Request;

public class AnalyzeRequest
{
    [JsonPropertyName("file")]
    public IFormFile? File { get; set; }

    [JsonPropertyName("query")]
    public string? Query { get; set; }
}