using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ModelConfig.Tenant;

public sealed class PayrollConfiguration : IEntityTypeConfiguration<Payroll>
{
    public void Configure(EntityTypeBuilder<Payroll> builder)
    {
        builder.ToTable("Payroll");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.IdEmployee)
            .IsRequired();

        builder.HasOne(x => x.Employee)
            .WithMany(e => e.Payrolls)
            .HasForeignKey(x => x.IdEmployee)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.PayrollPeriodCod)
            .IsRequired(false);

        builder.Property(x => x.EventName)
            .IsRequired(false)
            .HasMaxLength(200);

        builder.Property(x => x.EventAmount)
            .IsRequired(false)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.EventTypeName)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(x => x.ReferenceDate)
            .IsRequired(false)
            .HasColumnType("date");

        builder.Property(x => x.CalculationTypeName)
            .IsRequired(false)
            .HasMaxLength(200);

        builder.Property(x => x.EmployeeCodSeniorNumCad)
            .IsRequired(false);

        builder.Property(x => x.CollaboratorTypeCodeSeniorTipCol)
            .IsRequired(false);

        builder.Property(x => x.CompanyCodSeniorNumEmp)
            .IsRequired(false);

        builder.Property(x => x.PayrollPeriodCodSeniorCodCal)
            .IsRequired(false);

        builder.Property(x => x.EventTypeCodSeniorTipEve)
            .IsRequired(false);

        builder.Property(x => x.EventCodSeniorCodenv)
            .IsRequired(false);

        builder.Property(x => x.CalculationTypeCodSeniorTipCal)
            .IsRequired(false);

    }
}