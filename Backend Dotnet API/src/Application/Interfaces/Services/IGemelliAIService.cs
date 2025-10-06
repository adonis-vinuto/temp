using ErrorOr;
using System.Collections.Generic;
using System.IO;


namespace Application.Interfaces.Services;

public interface IGemelliAIService
{
    Task<ErrorOr<GemelliAIChatResponse>> ChatAsync(
        GemelliAIChatRequest request,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<string>> GetChatTitleAsync(
        string idSession,
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
    public string AgentType { get; set; } = string.Empty;
    public GemelliAIChatUser User { get; set; } = new();
    public Dictionary<string, string> Preferences { get; set; } = new();
    public List<string> Documents { get; set; } = new();
}

public class GemelliAIChatUser
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class GemelliAIChatResponse
{
    public string IdSession { get; set; } = string.Empty;
    public string MessageResponse { get; set; } = string.Empty;
    public GemelliAIChatUsage Usage { get; set; } = new();
}

public class GemelliAIChatUsage
{
    public Dictionary<string, GemelliAIChatUsageBreakdown> UsageBreakdownByModel { get; set; } = new();
    public GemelliAIChatUsageBreakdown GrandTotalUsage { get; set; } = new();
}

public class GemelliAIChatUsageBreakdown
{
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
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