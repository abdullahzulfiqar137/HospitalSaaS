using HospitalSaaS.Domain.Radiology;

namespace HospitalSaaS.Application.Radiology;

public sealed record RadiologyOrderDto(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    Guid? RadiologyTestId,
    string Procedure,
    DateTime OrderedAtUtc,
    RadiologyOrderStatus Status,
    string? Report
)
{
    public static RadiologyOrderDto From(RadiologyOrder order) =>
        new(
            order.Id,
            order.PatientId,
            order.DoctorId,
            order.RadiologyTestId,
            order.Procedure,
            order.OrderedAtUtc,
            order.Status,
            order.Report
        );
}

public sealed record RadiologyTestDto(
    Guid Id,
    string Code,
    string Name,
    string Modality,
    decimal Price,
    bool IsActive
)
{
    public static RadiologyTestDto From(RadiologyTest test) =>
        new(test.Id, test.Code, test.Name, test.Modality, test.Price, test.IsActive);
}

public sealed record RadiologyResultDto(Guid Id, string Report, int Version, DateTime RecordedAtUtc)
{
    public static RadiologyResultDto From(RadiologyResult result) =>
        new(result.Id, result.Report, result.Version, result.RecordedAtUtc);
}
