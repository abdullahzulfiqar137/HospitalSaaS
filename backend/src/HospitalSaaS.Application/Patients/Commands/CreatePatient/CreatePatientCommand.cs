using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Patients.Models;

namespace HospitalSaaS.Application.Patients.Commands.CreatePatient;

public sealed record CreatePatientCommand(
    string FirstName,
    string? MiddleName,
    string LastName,
    DateOnly DateOfBirth,
    string Gender,
    string? Phone,
    string? Email,
    string? NationalId,
    string? Address,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? MaritalStatus,
    string? Notes
) : ICommand<Result<PatientDto>>;
