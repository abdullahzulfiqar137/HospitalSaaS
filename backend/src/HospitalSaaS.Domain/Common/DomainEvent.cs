namespace HospitalSaaS.Domain.Common;

public abstract record DomainEvent(Guid Id, DateTime OccurredAtUtc)
{
    protected DomainEvent()
        : this(Guid.NewGuid(), DateTime.UtcNow) { }
}
