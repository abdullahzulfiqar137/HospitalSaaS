using HospitalSaaS.Domain.AuditLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSaaS.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.Property(x => x.Action).HasMaxLength(100);
        builder.Property(x => x.EntityName).HasMaxLength(200);
        builder.Property(x => x.EntityId).HasMaxLength(100);
        builder.Property(x => x.CorrelationId).HasMaxLength(100);
        builder.Property(x => x.IpAddress).HasMaxLength(64);
        builder.HasIndex(x => new { x.TenantId, x.OccurredAtUtc });
    }
}
