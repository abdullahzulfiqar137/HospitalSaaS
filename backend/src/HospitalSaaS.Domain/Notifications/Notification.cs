using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Notifications;

public sealed class Notification : TenantEntity
{
    private Notification() { }

    public Notification(Guid tenantId, Guid userId, string subject, string body, string channel)
        : base(tenantId)
    {
        UserId = userId;
        Subject = subject;
        Body = body;
        Channel = channel;
        Status = NotificationStatus.Pending;
    }

    public Guid UserId { get; private set; }
    public string Subject { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public string Channel { get; private set; } = string.Empty;
    public NotificationStatus Status { get; private set; }
    public DateTime? SentAtUtc { get; private set; }

    public void MarkSent()
    {
        Status = NotificationStatus.Sent;
        SentAtUtc = DateTime.UtcNow;
    }
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Failed,
}
