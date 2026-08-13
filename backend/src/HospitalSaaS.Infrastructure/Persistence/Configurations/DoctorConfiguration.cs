using HospitalSaaS.Domain.Doctors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSaaS.Infrastructure.Persistence.Configurations;

public sealed class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> b)
    {
        b.Property(x => x.ConsultationFee).HasPrecision(18, 2);
        b.HasIndex(x => new { x.TenantId, x.EmployeeNumber }).IsUnique();
        b.HasIndex(x => new { x.TenantId, x.LicenseNumber }).IsUnique();
        b.HasIndex(x => new
        {
            x.TenantId,
            x.DepartmentId,
            x.Status,
        });
    }
}
