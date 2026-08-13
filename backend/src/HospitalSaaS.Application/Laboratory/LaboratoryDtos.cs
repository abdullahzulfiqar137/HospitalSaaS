using HospitalSaaS.Domain.Laboratory;

namespace HospitalSaaS.Application.Laboratory;

public sealed record LabOrderItemDto(
    Guid Id,
    Guid? LabTestId,
    string TestCode,
    string TestName,
    string? Result,
    string? Unit,
    string? ReferenceRange
);

public sealed record LabOrderDto(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    DateTime OrderedAtUtc,
    LabOrderStatus Status,
    IReadOnlyList<LabOrderItemDto> Items
)
{
    public static LabOrderDto From(LabOrder x) =>
        new(
            x.Id,
            x.PatientId,
            x.DoctorId,
            x.OrderedAtUtc,
            x.Status,
            x.Items.Select(i => new LabOrderItemDto(
                    i.Id,
                    i.LabTestId,
                    i.TestCode,
                    i.TestName,
                    i.Result,
                    i.Unit,
                    i.ReferenceRange
                ))
                .ToList()
        );
}

public sealed record LabTestDto(
    Guid Id,
    string Name,
    string Code,
    string Category,
    decimal Price,
    string? NormalRange,
    string? Unit,
    bool IsActive
)
{
    public static LabTestDto From(LabTest test) =>
        new(
            test.Id,
            test.Name,
            test.Code,
            test.Category,
            test.Price,
            test.NormalRange,
            test.Unit,
            test.IsActive
        );
}

public sealed record LabResultDto(
    Guid Id,
    Guid LabOrderItemId,
    string Value,
    string? Unit,
    string? ReferenceRange,
    int Version,
    DateTime RecordedAtUtc
)
{
    public static LabResultDto From(LabResult result) =>
        new(
            result.Id,
            result.LabOrderItemId,
            result.Value,
            result.Unit,
            result.ReferenceRange,
            result.Version,
            result.RecordedAtUtc
        );
}
