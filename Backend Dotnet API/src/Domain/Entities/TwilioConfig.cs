namespace Domain.Entities;

public sealed class TwilioConfig : BaseEntity
{
    public Guid IdAgent { get; set; }
    public Agent Agent { get; set; }
    public string AccountSid { get; set; }
    public string AuthToken { get; set; }
    public string WebhookUrl { get; set; }
}