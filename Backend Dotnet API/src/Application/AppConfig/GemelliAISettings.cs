namespace Application.AppConfig;

public class GemelliAISettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 90;
    public int RetryCount { get; set; } = 3;
    public string Token { get; set; } = string.Empty;
}