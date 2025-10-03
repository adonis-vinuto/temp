using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ModelConfig.Tenant;

public sealed class SeniorHcmConfigConfiguration : IEntityTypeConfiguration<SeniorHcmConfig>
{
    public void Configure(EntityTypeBuilder<SeniorHcmConfig> builder)
    {
        builder.ToTable("SeniorHcmConfig");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Password)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.WsdlUrl)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.HasMany(x => x.Agents)
            .WithMany(x => x.SeniorHcmConfigs);
    }
}