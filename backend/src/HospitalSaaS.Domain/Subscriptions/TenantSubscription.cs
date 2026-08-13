using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Subscriptions;

public sealed class TenantSubscription : TenantEntity
{
    private TenantSubscription() { }

    public TenantSubscription(Guid tenantId, string plan, DateTime startsAtUtc, DateTime endsAtUtc)
        : base(tenantId)
    {
        Plan = plan;
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        Status = SubscriptionStatus.Active;
    }

    public string Plan { get; private set; } = string.Empty;
    public DateTime StartsAtUtc { get; private set; }
    public DateTime EndsAtUtc { get; private set; }
    public SubscriptionStatus Status { get; private set; }
}

public enum SubscriptionStatus
{
    Trial,
    Active,
    PastDue,
    Cancelled,
    Expired,
}
