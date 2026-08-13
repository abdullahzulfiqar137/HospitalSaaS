using HospitalSaaS.Domain.Patients;

namespace HospitalSaaS.Application.Patients.Models;

public sealed record PatientDto(
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
    PatientStatus Status
)
{
    public static PatientDto From(Patient x) =>
        new(
            x.Id,
            x.PatientNumber,
            x.FirstName,
            x.MiddleName,
            x.LastName,
            x.DateOfBirth,
            x.Gender,
            x.BloodGroup,
            x.Phone,
            x.Email,
            x.NationalId,
            x.Address,
            x.EmergencyContactName,
            x.EmergencyContactPhone,
            x.MaritalStatus,
            x.RegistrationDateUtc,
            x.Notes,
            x.Status
        );
}
