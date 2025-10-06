using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ModelConfig.Master;

public class LogEntityConfig : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.ToTable("Logs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(e => e.IdUser)
            .HasColumnType("char(36)");

        builder.Property(x => x.CreatedAt)
            .IsRequired();
            
        builder.Property(x => x.LastState).HasColumnType("TEXT").IsRequired();
        builder.Property(x => x.NewState).HasColumnType("TEXT").IsRequired();
    }
}