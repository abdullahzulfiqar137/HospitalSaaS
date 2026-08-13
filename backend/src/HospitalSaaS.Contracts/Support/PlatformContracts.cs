namespace HospitalSaaS.Contracts.Support;

public sealed record CreateStaffRequest(
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string JobTitle,
    Guid? DepartmentId
);

public sealed record StaffResponse(
    Guid Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string JobTitle,
    Guid? DepartmentId,
    bool IsActive
);

public sealed record CreateSubscriptionRequest(
    string Plan,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc
);

public sealed record SubscriptionResponse(
    Guid Id,
    string Plan,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    string Status
);

public sealed record DocumentResponse(
    Guid Id,
    Guid PatientId,
    string Name,
    string ContentType,
    long Size
);

public sealed record AuditLogResponse(
    Guid Id,
    Guid? UserId,
    string Action,
    string EntityName,
    string? EntityId,
    string? CorrelationId,
    string? OldValues,
    string? NewValues,
    string? IpAddress,
    DateTime OccurredAtUtc
);

public sealed record DashboardResponse(
    int Patients,
    int Doctors,
    int AppointmentsToday,
    int ActiveAdmissions,
    int UnpaidInvoices,
    int PendingLabOrders,
    int LowStockMedicines,
    int PendingNotifications
);
