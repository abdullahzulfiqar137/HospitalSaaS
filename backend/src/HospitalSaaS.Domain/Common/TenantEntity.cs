namespace HospitalSaaS.Domain.Common;

public abstract class TenantEntity : AuditableEntity
{
    protected TenantEntity() { }

    protected TenantEntity(Guid tenantId) => TenantId = tenantId;

    public Guid TenantId { get; private set; }
}
