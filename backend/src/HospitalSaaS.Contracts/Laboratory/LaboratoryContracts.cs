namespace HospitalSaaS.Contracts.Laboratory;

public sealed record CreateLabOrderRequest(
    Guid PatientId,
    Guid DoctorId,
    IReadOnlyList<LabTestRequest> Tests
);

public sealed record LabTestRequest(string Code, string Name, Guid? LabTestId = null);

public sealed record CreateLabTestRequest(
    string Name,
    string Code,
    string Category,
    decimal Price,
    string? NormalRange,
    string? Unit
);

public sealed record LabOrderActionRequest(string Action);

public sealed record RecordLabResultRequest(
    Guid ItemId,
    string Result,
    string? Unit,
    string? ReferenceRange
);
