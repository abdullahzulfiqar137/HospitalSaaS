using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Documents;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IDocumentRepository
{
    Task AddAsync(Document x, CancellationToken ct);
    Task<Document?> GetAsync(Guid id, bool tracking, CancellationToken ct);
    Task<PagedResult<Document>> SearchAsync(
        Guid? patientId,
        int page,
        int size,
        CancellationToken ct
    );
    void Remove(Document x);
    Task SaveChangesAsync(CancellationToken ct);
}
