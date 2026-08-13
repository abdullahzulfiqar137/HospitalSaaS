using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Patients;

public sealed class Patient : TenantEntity
{
    private Patient() { }

    public Patient(
        Guid tenantId,
        string patientNumber,
        string firstName,
        string? middleName,
        string lastName,
        DateOnly dateOfBirth,
        string gender,
        string? phone,
        string? email,
        string? nationalId,
        string? address,
        string? emergencyContactName,
        string? emergencyContactPhone,
        string? maritalStatus,
        string? notes
    )
        : base(tenantId)
    {
        if (dateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException("Date of birth cannot be in the future.");
        PatientNumber = patientNumber;
        FirstName = firstName.Trim();
        MiddleName = middleName?.Trim();
        LastName = lastName.Trim();
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Phone = phone;
        Email = email?.Trim().ToLowerInvariant();
        NationalId = nationalId;
        Address = address;
        EmergencyContactName = emergencyContactName;
        EmergencyContactPhone = emergencyContactPhone;
        MaritalStatus = maritalStatus;
        Notes = notes;
        RegistrationDateUtc = DateTime.UtcNow;
        Status = PatientStatus.Active;
    }

    public string PatientNumber { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; } = string.Empty;
    public DateOnly DateOfBirth { get; private set; }
    public string Gender { get; private set; } = string.Empty;
    public string? BloodGroup { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? NationalId { get; private set; }
    public string? Address { get; private set; }
    public string? EmergencyContactName { get; private set; }
    public string? EmergencyContactPhone { get; private set; }
    public string? MaritalStatus { get; private set; }
    public DateTime RegistrationDateUtc { get; private set; }
    public string? Notes { get; private set; }
    public PatientStatus Status { get; private set; }

    public void Update(
        string firstName,
        string? middleName,
        string lastName,
        DateOnly dateOfBirth,
        string gender,
        string? bloodGroup,
        string? phone,
        string? email,
        string? nationalId,
        string? address,
        string? emergencyName,
        string? emergencyPhone,
        string? maritalStatus,
        string? notes
    )
    {
        if (dateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException("Date of birth cannot be in the future.");
        FirstName = firstName.Trim();
        MiddleName = middleName?.Trim();
        LastName = lastName.Trim();
        DateOfBirth = dateOfBirth;
        Gender = gender;
        BloodGroup = bloodGroup;
        Phone = phone;
        Email = email?.Trim().ToLowerInvariant();
        NationalId = nationalId;
        Address = address;
        EmergencyContactName = emergencyName;
        EmergencyContactPhone = emergencyPhone;
        MaritalStatus = maritalStatus;
        Notes = notes;
    }
}

public enum PatientStatus
{
    Active,
    Inactive,
    Deceased,
}
