using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Users;

public sealed class RefreshToken : TenantEntity
{
    private RefreshToken() { }

    public RefreshToken(Guid tenantId, Guid userId, string tokenHash, DateTime expiresAtUtc)
        : base(tenantId)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
    }

    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }

    public bool IsValid(DateTime utcNow) => RevokedAtUtc is null && ExpiresAtUtc > utcNow;

    public void Revoke(DateTime utcNow) => RevokedAtUtc = utcNow;
}
