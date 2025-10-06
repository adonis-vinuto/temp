using Domain.Enums;

namespace Domain.Entities;

public sealed class ChatSession : BaseEntity
{
    public Guid IdAgent { get; set; }
    public Agent Agent { get; set; }
    public int TotalInteractions { get; set; }
    public string IdUser { get; set; }
    public string? IdSession { get; set; }
    public string? Title { get; set; }
    public DateTime LastSendDate { get; set; }
    public IEnumerable<ChatHistory> ChatHistory => _chatsHistories;
    private readonly List<ChatHistory> _chatsHistories =
        new();
}