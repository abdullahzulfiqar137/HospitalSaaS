using HospitalSaaS.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSaaS.Infrastructure.Persistence.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.RefundedAmount).HasPrecision(18, 2);
        b.HasIndex(x => new { x.TenantId, x.PaymentNumber }).IsUnique();
    }
}
