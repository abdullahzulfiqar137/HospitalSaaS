namespace HospitalSaaS.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAtUtc { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public Guid? DeletedBy { get; private set; }

    public void MarkCreated(DateTime utcNow, Guid? userId)
    {
        CreatedAtUtc = utcNow;
        CreatedBy = userId;
    }

    public void MarkUpdated(DateTime utcNow, Guid? userId)
    {
        UpdatedAtUtc = utcNow;
        UpdatedBy = userId;
    }

    public void SoftDelete(DateTime utcNow, Guid? userId)
    {
        IsDeleted = true;
        DeletedAtUtc = utcNow;
        DeletedBy = userId;
    }
}
