using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.AuditLogs;

public sealed class AuditLog : BaseEntity
{
    private AuditLog() { }

    public AuditLog(
        Guid? tenantId,
        Guid? userId,
        string action,
        string entityName,
        string? entityId,
        string? correlationId,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null
    )
        : base()
    {
        TenantId = tenantId;
        UserId = userId;
        Action = action;
        EntityName = entityName;
        EntityId = entityId;
        CorrelationId = correlationId;
        OldValues = oldValues;
        NewValues = newValues;
        IpAddress = ipAddress;
        OccurredAtUtc = DateTime.UtcNow;
    }

    public Guid? TenantId { get; private set; }
    public Guid? UserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string EntityName { get; private set; } = string.Empty;
    public string? EntityId { get; private set; }
    public string? CorrelationId { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }
}
