using HospitalSaaS.Domain.Admissions;
using HospitalSaaS.Domain.Inventory;
using HospitalSaaS.Domain.Laboratory;
using HospitalSaaS.Domain.MedicalRecords;
using HospitalSaaS.Domain.Radiology;
using HospitalSaaS.Domain.Suppliers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalSaaS.Infrastructure.Persistence.Configurations;

public sealed class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
{
    public void Configure(EntityTypeBuilder<MedicalRecord> builder) =>
        builder.HasMany(x => x.Diagnoses).WithOne().HasForeignKey(x => x.MedicalRecordId);
}

public sealed class AdmissionHistoryConfiguration : IEntityTypeConfiguration<AdmissionHistory>
{
    public void Configure(EntityTypeBuilder<AdmissionHistory> builder)
    {
        builder.HasIndex(x => new
        {
            x.TenantId,
            x.AdmissionId,
            x.TransferredAtUtc,
        });
        builder.Property(x => x.Reason).HasMaxLength(500);
        builder
            .HasOne<Admission>()
            .WithMany()
            .HasForeignKey(x => x.AdmissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class LabTestConfiguration : IEntityTypeConfiguration<LabTest>
{
    public void Configure(EntityTypeBuilder<LabTest> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Code).HasMaxLength(50);
        builder.Property(x => x.Category).HasMaxLength(100);
        builder.Property(x => x.NormalRange).HasMaxLength(200);
        builder.Property(x => x.Unit).HasMaxLength(100);
        builder.Property(x => x.Price).HasPrecision(18, 2);
    }
}

public sealed class LabOrderItemConfiguration : IEntityTypeConfiguration<LabOrderItem>
{
    public void Configure(EntityTypeBuilder<LabOrderItem> builder) =>
        builder
            .HasOne<LabTest>()
            .WithMany()
            .HasForeignKey(x => x.LabTestId)
            .OnDelete(DeleteBehavior.Restrict);
}

public sealed class LabResultConfiguration : IEntityTypeConfiguration<LabResult>
{
    public void Configure(EntityTypeBuilder<LabResult> builder)
    {
        builder
            .HasIndex(x => new
            {
                x.TenantId,
                x.LabOrderItemId,
                x.Version,
            })
            .IsUnique();
        builder.Property(x => x.Value).HasMaxLength(4000);
        builder.Property(x => x.Unit).HasMaxLength(100);
        builder.Property(x => x.ReferenceRange).HasMaxLength(200);
        builder
            .HasOne<LabOrder>()
            .WithMany()
            .HasForeignKey(x => x.LabOrderId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne<LabOrderItem>()
            .WithMany()
            .HasForeignKey(x => x.LabOrderItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class RadiologyTestConfiguration : IEntityTypeConfiguration<RadiologyTest>
{
    public void Configure(EntityTypeBuilder<RadiologyTest> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        builder.Property(x => x.Code).HasMaxLength(50);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.Modality).HasMaxLength(100);
        builder.Property(x => x.Price).HasPrecision(18, 2);
    }
}

public sealed class RadiologyOrderConfiguration : IEntityTypeConfiguration<RadiologyOrder>
{
    public void Configure(EntityTypeBuilder<RadiologyOrder> builder) =>
        builder
            .HasOne<RadiologyTest>()
            .WithMany()
            .HasForeignKey(x => x.RadiologyTestId)
            .OnDelete(DeleteBehavior.Restrict);
}

public sealed class RadiologyResultConfiguration : IEntityTypeConfiguration<RadiologyResult>
{
    public void Configure(EntityTypeBuilder<RadiologyResult> builder)
    {
        builder
            .HasIndex(x => new
            {
                x.TenantId,
                x.RadiologyOrderId,
                x.Version,
            })
            .IsUnique();
        builder.Property(x => x.Report).HasMaxLength(16000);
        builder
            .HasOne<RadiologyOrder>()
            .WithMany()
            .HasForeignKey(x => x.RadiologyOrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.HasIndex(x => new { x.TenantId, x.OrderNumber }).IsUnique();
        builder.Property(x => x.OrderNumber).HasMaxLength(50);
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.PurchaseOrderId);
        builder
            .HasOne<Supplier>()
            .WithMany()
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.Property(x => x.UnitCost).HasPrecision(18, 2);
        builder
            .HasOne<InventoryItem>()
            .WithMany()
            .HasForeignKey(x => x.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
