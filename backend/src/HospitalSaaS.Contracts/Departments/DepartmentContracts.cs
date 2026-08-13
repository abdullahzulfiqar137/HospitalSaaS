namespace HospitalSaaS.Contracts.Departments;

public sealed record CreateDepartmentRequest(string Name, string Code, string? Description);

public sealed record UpdateDepartmentRequest(
    string Name,
    string Code,
    string? Description,
    Guid? HeadDoctorId
);

public sealed record DepartmentResponse(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    string Status
);
