using HospitalSaaS.Domain.Common;

namespace HospitalSaaS.Domain.Staff;

public sealed class StaffMember : TenantEntity
{
    private StaffMember() { }

    public StaffMember(
        Guid tenantId,
        string employeeNumber,
        string firstName,
        string lastName,
        string jobTitle,
        Guid? departmentId
    )
        : base(tenantId)
    {
        EmployeeNumber = employeeNumber;
        FirstName = firstName;
        LastName = lastName;
        JobTitle = jobTitle;
        DepartmentId = departmentId;
        IsActive = true;
    }

    public Guid? UserId { get; private set; }
    public string EmployeeNumber { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string JobTitle { get; private set; } = string.Empty;
    public Guid? DepartmentId { get; private set; }
    public bool IsActive { get; private set; }
}
