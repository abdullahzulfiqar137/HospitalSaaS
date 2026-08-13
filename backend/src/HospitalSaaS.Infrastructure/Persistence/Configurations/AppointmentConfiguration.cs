using HospitalSaaS.Domain.Appointments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSaaS.Infrastructure.Persistence.Configurations;

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> b)
    {
        b.HasIndex(x => new
        {
            x.TenantId,
            x.DoctorId,
            x.AppointmentDateTimeUtc,
        });
        b.HasIndex(x => new
        {
            x.TenantId,
            x.PatientId,
            x.AppointmentDateTimeUtc,
        });
        b.HasIndex(x => new { x.TenantId, x.Status });
        b.Property(x => x.Reason).HasMaxLength(1000);
    }
}
