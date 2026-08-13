namespace HospitalSaaS.Application.Common.Interfaces;

public interface ITenantContext
{
    Guid TenantId { get; }
    bool HasTenant { get; }
    bool IsFilterBypassed { get; }
    void SetTenant(Guid tenantId);
    IDisposable BeginSuperAdminBypass(bool authorized);
}
