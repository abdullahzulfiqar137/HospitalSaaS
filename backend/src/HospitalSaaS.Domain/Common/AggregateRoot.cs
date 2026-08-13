namespace HospitalSaaS.Domain.Common;

public abstract class AggregateRoot : TenantEntity
{
    private readonly List<DomainEvent> _domainEvents = [];

    protected AggregateRoot() { }

    protected AggregateRoot(Guid tenantId)
        : base(tenantId) { }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents;

    protected void Raise(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
