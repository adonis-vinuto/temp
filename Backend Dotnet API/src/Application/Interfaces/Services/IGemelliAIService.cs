using System.Collections.Generic;
using ErrorOr;

namespace Application.Interfaces.Services;

public interface IGemelliAIService
{
    Task<ErrorOr<GemelliAIChatResponse>> ChatAsync(
        GemelliAIChatRequest request,
        CancellationToken cancellationToken = default);


    Task<ErrorOr<GemelliAIFileResponse>> FileAsync(
        GemelliAIFileRequest request,
        CancellationToken cancellationToken = default);
}

public class GemelliAIChatRequest
{
    public string? IdSession { get; set; }
    public string IdAgent { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string AgentType { get; set; } = string.Empty;
    public Dictionary<string, string>? Preferences { get; set; }
    public List<string> Documents { get; set; } = new();
    public List<(int Role, string Content)> ChatHistory { get; set; } = new();
}

public class GemelliAIChatResponse
{
    public string MessageResponse { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public int TotalTokens { get; set; }
}

public class GemelliAIFileResponse
{
    public string FileName { get; set; } = string.Empty;
    public string Resume { get; set; } = string.Empty;
}

public class GemelliAIFileRequest
{
    public Stream FileStream { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public string IdAgent { get; set; } = string.Empty;
}