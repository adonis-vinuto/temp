using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ModelConfig.Tenant;

public sealed class ChatHistoryConfiguration : IEntityTypeConfiguration<ChatHistory>
{
    public void Configure(EntityTypeBuilder<ChatHistory> builder)
    {

        builder.ToTable("ChatHistory");
        
        builder.HasKey(ch => ch.Id);
        
        builder.HasOne(ch => ch.ChatSession)
            .WithMany(cs => cs.ChatHistory)
            .HasForeignKey(ch => ch.IdChatSession)
            .OnDelete(DeleteBehavior.ClientSetNull);
        
        builder.Property(ch => ch.Content)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(ch => ch.Role)
            .IsRequired();
        
        builder.Property(cs => cs.IdUser)
            .IsRequired();

        builder.Property(ch => ch.TotalTokens)
            .IsRequired();

        builder.Property(ch => ch.TotalTime)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}