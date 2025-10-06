using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Infrastructure.Contracts.GemelliAI.Response;

public class ChatResponse
{
    [JsonPropertyName("id_session")]
    public string IdSession { get; set; } = string.Empty;

    [JsonPropertyName("message-response")]
    public string MessageResponse { get; set; } = string.Empty;

    [JsonPropertyName("usage")]
    public ChatUsage Usage { get; set; } = new();
}

public class ChatUsage
{
    [JsonPropertyName("usage_breakdown_by_model")]
    public Dictionary<string, ChatUsageMetrics> UsageBreakdownByModel { get; set; } = new();

    [JsonPropertyName("grand_total_usage")]
    public ChatUsageMetrics GrandTotalUsage { get; set; } = new();
}

public class ChatUsageMetrics
{
    [JsonPropertyName("input_tokens")]
    public int InputTokens { get; set; }

    [JsonPropertyName("output_tokens")]
    public int OutputTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}
