using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Suppliers;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface ISupplierRepository
{
    Task AddAsync(Supplier x, CancellationToken ct);
    Task<Supplier?> GetAsync(Guid id, bool tracking, CancellationToken ct);
    Task<PagedResult<Supplier>> SearchAsync(
        string? search,
        int page,
        int size,
        CancellationToken ct
    );
    void Remove(Supplier x);
    Task SaveChangesAsync(CancellationToken ct);
}
