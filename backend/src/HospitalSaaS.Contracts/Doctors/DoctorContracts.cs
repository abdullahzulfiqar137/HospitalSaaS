namespace HospitalSaaS.Contracts.Doctors;

public sealed record CreateDoctorRequest(
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Specialization,
    Guid DepartmentId,
    string LicenseNumber,
    string? Phone,
    string? Email,
    decimal ConsultationFee
);

public sealed record UpdateDoctorRequest(
    string FirstName,
    string LastName,
    string Specialization,
    Guid DepartmentId,
    string LicenseNumber,
    string? Phone,
    string? Email,
    decimal ConsultationFee
);

public sealed record DoctorResponse(
    Guid Id,
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Specialization,
    Guid DepartmentId,
    string LicenseNumber,
    string? Phone,
    string? Email,
    decimal ConsultationFee,
    string Status
);
