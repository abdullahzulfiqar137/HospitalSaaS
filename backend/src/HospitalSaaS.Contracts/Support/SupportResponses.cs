namespace HospitalSaaS.Contracts.Support;

public sealed record SupplierResponse(
    Guid Id,
    string Name,
    string? ContactName,
    string? Phone,
    string? Email,
    string? Address,
    bool IsActive
);

public sealed record InsurancePolicyResponse(
    Guid Id,
    Guid PatientId,
    string Provider,
    string PolicyNumber,
    DateOnly ValidFrom,
    DateOnly ValidTo
);

public sealed record NotificationResponse(
    Guid Id,
    Guid UserId,
    string Subject,
    string Channel,
    string Status,
    DateTime? SentAtUtc
);
