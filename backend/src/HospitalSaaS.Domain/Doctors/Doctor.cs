using HospitalSaaS.Domain.Common;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Doctors;

public sealed class Doctor : TenantEntity
{
    private Doctor() { }

    public Doctor(
        Guid tenantId,
        string employeeNumber,
        string firstName,
        string lastName,
        string specialization,
        Guid departmentId,
        string licenseNumber,
        string? phone,
        string? email,
        decimal consultationFee
    )
        : base(tenantId)
    {
        EmployeeNumber = employeeNumber.Trim();
        Update(
            firstName,
            lastName,
            specialization,
            departmentId,
            licenseNumber,
            phone,
            email,
            consultationFee
        );
        Status = DoctorStatus.Active;
    }

    public Guid? UserId { get; private set; }
    public string EmployeeNumber { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Specialization { get; private set; } = string.Empty;
    public Guid DepartmentId { get; private set; }
    public string LicenseNumber { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public decimal ConsultationFee { get; private set; }
    public DoctorStatus Status { get; private set; }

    public void Update(
        string firstName,
        string lastName,
        string specialization,
        Guid departmentId,
        string licenseNumber,
        string? phone,
        string? email,
        decimal fee
    )
    {
        if (fee < 0)
            throw new DomainException("Consultation fee cannot be negative.");
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Specialization = specialization.Trim();
        DepartmentId = departmentId;
        LicenseNumber = licenseNumber.Trim();
        Phone = phone;
        Email = email?.Trim().ToLowerInvariant();
        ConsultationFee = fee;
    }

    public void Activate() => Status = DoctorStatus.Active;

    public void Deactivate() => Status = DoctorStatus.Inactive;
}

public enum DoctorStatus
{
    Active,
    Inactive,
}
