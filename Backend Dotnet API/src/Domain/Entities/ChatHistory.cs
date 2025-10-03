using Domain.Enums;

namespace Domain.Entities;

public sealed class ChatHistory : BaseEntity
{
    public Guid IdChatSession { get; set; }
    public ChatSession ChatSession { get; set; }
    public string Content { get; set; }
    public string IdUser { get; set; }
    public RoleChat Role { get; set; }
    public int TotalTokens { get; set; }
    public decimal TotalTime { get; set; }
}