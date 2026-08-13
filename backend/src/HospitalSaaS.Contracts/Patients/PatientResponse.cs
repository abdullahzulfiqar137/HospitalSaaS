namespace HospitalSaaS.Contracts.Patients;

public sealed record PatientResponse(
    Guid Id,
    string PatientNumber,
    string FirstName,
    string? MiddleName,
    string LastName,
    DateOnly DateOfBirth,
    string Gender,
    string? BloodGroup,
    string? Phone,
    string? Email,
    string? NationalId,
    string? Address,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? MaritalStatus,
    DateTime RegistrationDateUtc,
    string? Notes,
    string Status
);
