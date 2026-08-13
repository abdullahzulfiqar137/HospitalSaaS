using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Tenants;

public sealed class TenantSettings : TenantEntity
{
    private TenantSettings() { }

    public TenantSettings(Guid tenantId, string timeZone, string currency)
        : base(tenantId)
    {
        TimeZone = timeZone;
        Currency = currency;
    }

    public string TimeZone { get; private set; } = string.Empty;
    public string Currency { get; private set; } = string.Empty;
    public string? LogoStorageKey { get; private set; }
}
