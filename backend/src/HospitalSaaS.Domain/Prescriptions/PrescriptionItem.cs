using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Prescriptions;

public sealed class PrescriptionItem : BaseEntity
{
    private PrescriptionItem() { }

    internal PrescriptionItem(
        Guid prescriptionId,
        Guid medicineId,
        string dose,
        string frequency,
        string duration,
        string route,
        string? instructions
    )
    {
        PrescriptionId = prescriptionId;
        MedicineId = medicineId;
        Dose = dose;
        Frequency = frequency;
        Duration = duration;
        Route = route;
        Instructions = instructions;
    }

    public Guid PrescriptionId { get; private set; }
    public Guid MedicineId { get; private set; }
    public string Dose { get; private set; } = string.Empty;
    public string Frequency { get; private set; } = string.Empty;
    public string Duration { get; private set; } = string.Empty;
    public string Route { get; private set; } = string.Empty;
    public string? Instructions { get; private set; }
}
