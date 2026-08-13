using HospitalSaaS.Application.Common.Interfaces;

namespace HospitalSaaS.Infrastructure.Tenancy;

public sealed class TenantContext : ITenantContext
{
    private int _bypassDepth;
    public Guid TenantId { get; private set; }
    public bool HasTenant => TenantId != Guid.Empty;
    public bool IsFilterBypassed => _bypassDepth > 0;

    public void SetTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.");
        if (HasTenant && TenantId != tenantId)
            throw new InvalidOperationException("Tenant cannot change during a request.");
        TenantId = tenantId;
    }

    public IDisposable BeginSuperAdminBypass(bool authorized)
    {
        if (!authorized)
            throw new UnauthorizedAccessException("SaaS super-admin authorization is required.");
        _bypassDepth++;
        return new Scope(() => _bypassDepth--);
    }

    private sealed class Scope(Action dispose) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                dispose();
                _disposed = true;
            }
        }
    }
}
