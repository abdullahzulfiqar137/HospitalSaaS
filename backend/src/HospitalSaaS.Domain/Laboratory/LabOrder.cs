using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Laboratory;

public sealed class LabOrder : AggregateRoot
{
    private readonly List<LabOrderItem> _items = [];

    private LabOrder() { }

    public LabOrder(Guid tenantId, Guid patientId, Guid doctorId)
        : base(tenantId)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        OrderedAtUtc = DateTime.UtcNow;
        Status = LabOrderStatus.Ordered;
    }

    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public DateTime OrderedAtUtc { get; private set; }
    public LabOrderStatus Status { get; private set; }
    public IReadOnlyCollection<LabOrderItem> Items => _items;

    public void AddTest(string code, string name, Guid? labTestId = null) =>
        _items.Add(new(Id, labTestId, code, name));

    public void MarkSampleCollected()
    {
        if (Status != LabOrderStatus.Ordered)
            throw new DomainException("Only ordered lab work can be collected.");
        Status = LabOrderStatus.SampleCollected;
    }

    public void StartProcessing()
    {
        if (Status != LabOrderStatus.SampleCollected)
            throw new DomainException("A sample must be collected before processing.");
        Status = LabOrderStatus.Processing;
    }

    public void Cancel()
    {
        if (Status is LabOrderStatus.Completed or LabOrderStatus.Cancelled)
            throw new DomainException("The lab order cannot be cancelled.");
        Status = LabOrderStatus.Cancelled;
    }

    public void Complete()
    {
        if (_items.Count == 0 || _items.Any(x => x.Result is null))
            throw new DomainException("All test results must be recorded.");
        Status = LabOrderStatus.Completed;
    }
}

public enum LabOrderStatus
{
    Ordered,
    SampleCollected,
    Processing,
    Completed,
    Cancelled,
}
