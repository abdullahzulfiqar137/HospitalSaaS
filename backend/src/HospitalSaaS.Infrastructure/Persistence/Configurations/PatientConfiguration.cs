using HospitalSaaS.Domain.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSaaS.Infrastructure.Persistence.Configurations;

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> b)
    {
        b.ToTable("Patients");
        b.HasKey(x => x.Id);
        b.Property(x => x.PatientNumber).HasMaxLength(30).IsRequired();
        b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.MiddleName).HasMaxLength(100);
        b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        b.Property(x => x.Gender).HasMaxLength(30).IsRequired();
        b.Property(x => x.BloodGroup).HasMaxLength(10);
        b.Property(x => x.Phone).HasMaxLength(30);
        b.Property(x => x.Email).HasMaxLength(254);
        b.Property(x => x.NationalId).HasMaxLength(50);
        b.Property(x => x.Address).HasMaxLength(500);
        b.Property(x => x.Notes).HasMaxLength(2000);
        b.HasIndex(x => new { x.TenantId, x.PatientNumber }).IsUnique();
        b.HasIndex(x => new { x.TenantId, x.Email });
        b.HasIndex(x => new { x.TenantId, x.Phone });
        b.HasIndex(x => new { x.TenantId, x.CreatedAtUtc });
    }
}
