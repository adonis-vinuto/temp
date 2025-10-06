using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ModelConfig.Tenant;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employee");

        builder.HasKey(a => a.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(a => a.IdKnowledge)
            .IsRequired();

        builder.HasOne(e => e.Knowledge)
            .WithMany()
            .HasForeignKey(e => e.IdKnowledge)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.CompanyName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(a => a.FullName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(a => a.AdmissionDate)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(a => a.TerminationDate)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(a => a.StatusDescription)
            .IsRequired(false);

        builder.Property(a => a.BirthDate)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(a => a.CostCneterName)
            .IsRequired(false);

        builder.Property(a => a.Salary)
            .HasColumnType("decimal(15, 2)")
            .IsRequired(false);

        builder.Property(a => a.ComplementarySalary)
            .HasColumnType("decimal(15, 2)")
            .IsRequired(false);

        builder.Property(a => a.SalaryEffectiveDate)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(a => a.Gender)
            .IsRequired();

        builder.Property(a => a.StreetAddress)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(a => a.AddressNumber)
            .IsRequired(false);

        builder.Property(a => a.CityName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(a => a.Race)
            .IsRequired(false);

        builder.Property(a => a.PostalCode)
            .IsRequired(false);

        builder.Property(a => a.CompanyCodSeniorNumEmp)
            .IsRequired(false);

        builder.Property(a => a.EmployeeCodSeniorNumCad)
            .IsRequired(false);

        builder.Property(a => a.CollaboratorTypeCodeSeniorTipeCol)
            .IsRequired(false);

        builder.Property(a => a.StatusCodSenior)
            .IsRequired(false);

        builder.Property(a => a.CostCenterCodSeniorCodCcu)
            .IsRequired(false);

        builder.HasMany(e => e.SalaryHistories)
            .WithOne(sh => sh.Employee)
            .HasForeignKey(sh => sh.IdEmployee)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Payrolls)
            .WithOne(p => p.Employee)
            .HasForeignKey(p => p.IdEmployee)
            .OnDelete(DeleteBehavior.Cascade);
    }
}