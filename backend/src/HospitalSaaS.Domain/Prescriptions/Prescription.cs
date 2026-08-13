using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Prescriptions;

public sealed class Prescription : AggregateRoot
{
    private readonly List<PrescriptionItem> _items = [];

    private Prescription() { }

    public Prescription(
        Guid tenantId,
        Guid patientId,
        Guid doctorId,
        Guid? appointmentId,
        string? notes
    )
        : base(tenantId)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        AppointmentId = appointmentId;
        DateUtc = DateTime.UtcNow;
        Notes = notes;
    }

    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public Guid? AppointmentId { get; private set; }
    public DateTime DateUtc { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<PrescriptionItem> Items => _items;

    public void AddItem(
        Guid medicineId,
        string dose,
        string frequency,
        string duration,
        string route,
        string? instructions
    ) => _items.Add(new(Id, medicineId, dose, frequency, duration, route, instructions));
}
