using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ModelConfig.Tenant;

public sealed class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.ToTable("Agents");
        
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Description)
            .HasMaxLength(255);

        builder.Property(a => a.Type)
            .IsRequired();

        builder.Property(a => a.Module)
            .IsRequired();

        builder.Property(a => a.Organization)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();
        
        builder.HasMany(a => a.Chats)
            .WithOne(c => c.Agent)
            .HasForeignKey(c => c.IdAgent)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}