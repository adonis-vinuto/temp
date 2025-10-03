using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ModelConfig.Tenant;

public sealed class TwilioConfigConfiguration : IEntityTypeConfiguration<TwilioConfig>
{
    public void Configure(EntityTypeBuilder<TwilioConfig> builder)
    {
        builder.ToTable("TwilioConfig");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.IdAgent)
            .IsRequired();

        builder.HasOne(x => x.Agent)
            .WithMany()
            .HasForeignKey(x => x.IdAgent)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.AccountSid)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.AuthToken)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.WebhookUrl)
            .IsRequired()
            .HasMaxLength(1000);

    }
}