using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Staff;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IStaffRepository
{
    Task AddAsync(StaffMember x, CancellationToken ct);
    Task<bool> EmployeeNumberExistsAsync(string number, CancellationToken ct);
    Task<PagedResult<StaffMember>> SearchAsync(
        string? search,
        int page,
        int size,
        CancellationToken ct
    );
    Task SaveChangesAsync(CancellationToken ct);
}
