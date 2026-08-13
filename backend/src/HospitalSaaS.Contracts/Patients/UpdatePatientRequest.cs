namespace HospitalSaaS.Contracts.Patients;

public sealed record UpdatePatientRequest(
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
    string? Notes
);
