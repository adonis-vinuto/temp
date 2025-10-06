namespace Application.DTOs.TwilioConfig;

public class TwilioConfigResponse
{
    public Guid Id { get; set; }
    public Guid IdAgent { get; set; }
    public string AgentName { get; set; } = string.Empty;
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string WebhookUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}