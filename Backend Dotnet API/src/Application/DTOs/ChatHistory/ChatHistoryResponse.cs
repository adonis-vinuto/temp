using Domain.Enums;

namespace Application.DTOs.ChatHistory;

public class ChatHistoryResponse
{
   public RoleChat? Role { get; set; }
   public string? Content { get; set; }
   public string? SendDate { get; set; }
   public UsageDto? Usage { get; set; }
}

public class UsageDto
{
   public int TotalTokens { get; set; }
   public int TotalTime { get; set; }
}