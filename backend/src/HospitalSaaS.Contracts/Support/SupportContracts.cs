namespace HospitalSaaS.Contracts.Support;

public sealed record CreateSupplierRequest(
    string Name,
    string? ContactName,
    string? Phone,
    string? Email,
    string? Address
);

public sealed record CreateInsurancePolicyRequest(
    Guid PatientId,
    string Provider,
    string PolicyNumber,
    DateOnly ValidFrom,
    DateOnly ValidTo
);

public sealed record CreateNotificationRequest(
    Guid UserId,
    string Subject,
    string Body,
    string Channel
);
