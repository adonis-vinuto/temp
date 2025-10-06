namespace Application.DTOs.Chat;

public class FirstMessageResponse
{
    public string MessageResponse { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string ExternalSessionId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}
