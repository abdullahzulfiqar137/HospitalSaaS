using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Documents;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class DocumentRepository(HospitalDbContext db) : IDocumentRepository
{
    public Task AddAsync(Document x, CancellationToken ct) => db.Documents.AddAsync(x, ct).AsTask();

    public Task<Document?> GetAsync(Guid id, bool tracking, CancellationToken ct) =>
        (tracking ? db.Documents : db.Documents.AsNoTracking()).SingleOrDefaultAsync(
            x => x.Id == id,
            ct
        );

    public void Remove(Document x) => db.Documents.Remove(x);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    public async Task<PagedResult<Document>> SearchAsync(
        Guid? patientId,
        int page,
        int size,
        CancellationToken ct
    )
    {
        var q = db.Documents.AsNoTracking();
        if (patientId.HasValue)
            q = q.Where(x => x.PatientId == patientId);
        var c = await q.CountAsync(ct);
        return new(
            await q.OrderByDescending(x => x.CreatedAtUtc)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(ct),
            page,
            size,
            c
        );
    }
}
