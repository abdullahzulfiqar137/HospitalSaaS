namespace HospitalSaaS.Contracts.Common;

public sealed record ErrorResponse(
    string Code,
    string Message,
    string? CorrelationId,
    IDictionary<string, string[]>? Errors = null
);
