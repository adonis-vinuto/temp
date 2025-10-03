using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ModelConfig.Master;
public class DataConfigConfiguration : IEntityTypeConfiguration<DataConfig>
{
    public void Configure(EntityTypeBuilder<DataConfig> builder)
    {
        builder.ToTable("DataConfigs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Module)
            .IsRequired();

        builder.Property(x => x.Organization)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.SqlHost)
            .IsRequired()
            .HasMaxLength(150);
        
        builder.Property(x => x.SqlPort)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.SqlUser)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.SqlPassword)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.SqlDatabase)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.BlobConnectionString)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.BlobContainerName)
            .IsRequired()
            .HasMaxLength(100);
    }
}