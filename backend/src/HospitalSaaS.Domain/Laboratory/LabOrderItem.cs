using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Laboratory;

public sealed class LabOrderItem : BaseEntity
{
    private LabOrderItem() { }

    internal LabOrderItem(Guid orderId, Guid? labTestId, string code, string name)
    {
        LabOrderId = orderId;
        LabTestId = labTestId;
        TestCode = code;
        TestName = name;
    }

    public Guid LabOrderId { get; private set; }
    public Guid? LabTestId { get; private set; }
    public string TestCode { get; private set; } = string.Empty;
    public string TestName { get; private set; } = string.Empty;
    public string? Result { get; private set; }
    public string? Unit { get; private set; }
    public string? ReferenceRange { get; private set; }

    public void RecordResult(string result, string? unit, string? range)
    {
        Result = result;
        Unit = unit;
        ReferenceRange = range;
    }
}
