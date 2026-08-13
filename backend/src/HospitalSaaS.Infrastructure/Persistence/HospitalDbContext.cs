using System.Linq.Expressions;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Domain.Admissions;
using HospitalSaaS.Domain.Appointments;
using HospitalSaaS.Domain.AuditLogs;
using HospitalSaaS.Domain.Billing;
using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Departments;
using HospitalSaaS.Domain.Diagnoses;
using HospitalSaaS.Domain.Doctors;
using HospitalSaaS.Domain.Documents;
using HospitalSaaS.Domain.Insurance;
using HospitalSaaS.Domain.Inventory;
using HospitalSaaS.Domain.Laboratory;
using HospitalSaaS.Domain.MedicalRecords;
using HospitalSaaS.Domain.Notifications;
using HospitalSaaS.Domain.Patients;
using HospitalSaaS.Domain.Payments;
using HospitalSaaS.Domain.Pharmacy;
using HospitalSaaS.Domain.Prescriptions;
using HospitalSaaS.Domain.Radiology;
using HospitalSaaS.Domain.Roles;
using HospitalSaaS.Domain.Staff;
using HospitalSaaS.Domain.Subscriptions;
using HospitalSaaS.Domain.Suppliers;
using HospitalSaaS.Domain.Tenants;
using HospitalSaaS.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class HospitalDbContext(
    DbContextOptions<HospitalDbContext> options,
    ITenantContext tenant
) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantSettings> TenantSettings => Set<TenantSettings>();
    public DbSet<TenantSubscription> TenantSubscriptions => Set<TenantSubscription>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<StaffMember> StaffMembers => Set<StaffMember>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Ward> Wards => Set<Ward>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Bed> Beds => Set<Bed>();
    public DbSet<Admission> Admissions => Set<Admission>();
    public DbSet<AdmissionHistory> AdmissionHistories => Set<AdmissionHistory>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    public DbSet<Diagnosis> Diagnoses => Set<Diagnosis>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PrescriptionItem> PrescriptionItems => Set<PrescriptionItem>();
    public DbSet<Medicine> Medicines => Set<Medicine>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<LabOrder> LabOrders => Set<LabOrder>();
    public DbSet<LabOrderItem> LabOrderItems => Set<LabOrderItem>();
    public DbSet<LabTest> LabTests => Set<LabTest>();
    public DbSet<LabResult> LabResults => Set<LabResult>();
    public DbSet<RadiologyOrder> RadiologyOrders => Set<RadiologyOrder>();
    public DbSet<RadiologyTest> RadiologyTests => Set<RadiologyTest>();
    public DbSet<RadiologyResult> RadiologyResults => Set<RadiologyResult>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<InsurancePolicy> InsurancePolicies => Set<InsurancePolicy>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HospitalDbContext).Assembly);
        foreach (
            var entityType in modelBuilder
                .Model.GetEntityTypes()
                .Where(x => typeof(TenantEntity).IsAssignableFrom(x.ClrType))
        )
            entityType.SetQueryFilter(BuildTenantFilter(entityType.ClrType));
    }

    private Guid CurrentTenantId => tenant.TenantId;
    private bool HasTenant => tenant.HasTenant;
    private bool IsTenantFilterBypassed => tenant.IsFilterBypassed;

    private LambdaExpression BuildTenantFilter(Type entityType)
    {
        var entity = Expression.Parameter(entityType, "entity");
        var notDeleted = Expression.Equal(
            EfProperty<bool>(entity, nameof(AuditableEntity.IsDeleted)),
            Expression.Constant(false)
        );
        var context = Expression.Constant(this);
        var bypass = Expression.Property(context, nameof(IsTenantFilterBypassed));
        var hasTenant = Expression.Property(context, nameof(HasTenant));
        var matchingTenant = Expression.Equal(
            EfProperty<Guid>(entity, nameof(TenantEntity.TenantId)),
            Expression.Property(context, nameof(CurrentTenantId))
        );
        return Expression.Lambda(
            Expression.AndAlso(
                notDeleted,
                Expression.OrElse(bypass, Expression.AndAlso(hasTenant, matchingTenant))
            ),
            entity
        );
    }

    private static MethodCallExpression EfProperty<T>(Expression entity, string name) =>
        Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            [typeof(T)],
            entity,
            Expression.Constant(name)
        );
}
