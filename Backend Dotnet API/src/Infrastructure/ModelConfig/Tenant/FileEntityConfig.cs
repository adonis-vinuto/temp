using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FileEntity = Domain.Entities.File;

namespace Infrastructure.ModelConfig.Tenant;

public sealed class FileConfiguration : IEntityTypeConfiguration<FileEntity>
{
    public void Configure(EntityTypeBuilder<FileEntity> builder)
    {
        builder.ToTable("File");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(a => a.Module)
            .IsRequired();

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.CompletionTokens)
            .IsRequired(false);

        builder.Property(x => x.PromptTokens)
            .IsRequired(false);

        builder.Property(x => x.TotalTokens)
            .IsRequired(false);

        builder.Property(x => x.CompletionTime)
            .IsRequired(false);

        builder.Property(x => x.PromptTime)
            .IsRequired(false);

        builder.Property(x => x.QueueTime)
            .IsRequired(false);

        builder.Property(x => x.TotalTime)
            .IsRequired(false);
            
        builder.HasMany(x => x.Agents)
            .WithMany(x => x.Files);
    }
}