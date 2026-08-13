namespace HospitalSaaS.Contracts.Laboratory;

public sealed record LabOrderResponse(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    DateTime OrderedAtUtc,
    string Status,
    IReadOnlyList<LabOrderItemResponse> Items
);

public sealed record LabOrderItemResponse(
    Guid Id,
    Guid? LabTestId,
    string TestCode,
    string TestName,
    string? Result,
    string? Unit,
    string? ReferenceRange
);

public sealed record LabTestResponse(
    Guid Id,
    string Name,
    string Code,
    string Category,
    decimal Price,
    string? NormalRange,
    string? Unit,
    bool IsActive
);

public sealed record LabResultResponse(
    Guid Id,
    Guid LabOrderItemId,
    string Value,
    string? Unit,
    string? ReferenceRange,
    int Version,
    DateTime RecordedAtUtc
);
