using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Departments;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IDepartmentRepository
{
    Task AddAsync(Department department, CancellationToken cancellationToken);
    Task<Department?> GetAsync(Guid id, bool tracking, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(string code, Guid? excludingId, CancellationToken cancellationToken);
    Task<PagedResult<Department>> SearchAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken
    );
    void Remove(Department department);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
