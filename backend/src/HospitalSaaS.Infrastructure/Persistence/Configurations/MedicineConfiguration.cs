using HospitalSaaS.Domain.Pharmacy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSaaS.Infrastructure.Persistence.Configurations;

public sealed class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
{
    public void Configure(EntityTypeBuilder<Medicine> b)
    {
        b.Property(x => x.SalePrice).HasPrecision(18, 2);
        b.Property(x => x.PurchasePrice).HasPrecision(18, 2);
        b.HasIndex(x => new { x.TenantId, x.Barcode })
            .IsUnique()
            .HasFilter("[Barcode] IS NOT NULL");
    }
}
