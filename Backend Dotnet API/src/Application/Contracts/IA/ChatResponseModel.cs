using System.Security.Cryptography;

namespace Application.Contracts.IA;

public record ChatResponseModel
{
    public string? MessageResponse { get; set; }
    public UsageResponseModel Usage { get; set; }
}

public record UsageResponseModel
{
    public string? ModelName { get; set; }
    public string? FinishReason { get; set; }
    public int? InputTokens { get; set; }
    public int? OutputTokens { get; set; }
    public int? TotalTokens { get; set; }
}