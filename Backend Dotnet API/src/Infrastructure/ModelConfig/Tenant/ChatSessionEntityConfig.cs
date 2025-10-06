using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ModelConfig.Tenant;

public sealed class ChatSessionConfiguration : IEntityTypeConfiguration<ChatSession>
{
    public void Configure(EntityTypeBuilder<ChatSession> builder)
    {
        builder.ToTable("ChatSessions");

        builder.HasKey(cs => cs.Id);

        builder.Property(cs => cs.TotalInteractions)
            .IsRequired();

        builder.Property(cs => cs.LastSendDate)
            .IsRequired();

        builder.Property(cs => cs.IdAgent)
            .IsRequired();

        builder.Property(cs => cs.IdUser)
            .IsRequired();

        builder.Property(cs => cs.IdSession)
            .HasMaxLength(200);

        builder.Property(cs => cs.Title)
            .HasMaxLength(400);

        builder.HasOne(cs => cs.Agent)
            .WithMany(a => a.Chats)
            .HasForeignKey(cs => cs.IdAgent)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cs => cs.ChatHistory)
            .WithOne(h => h.ChatSession)
            .HasForeignKey(h => h.IdChatSession)
            .OnDelete(DeleteBehavior.Cascade);
    }
}