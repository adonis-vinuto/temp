using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Contracts.IA.Pdf.Request;

public class DocSortRequest
{
    [JsonPropertyName("file")]
    public IFormFile? File { get; set; }
}