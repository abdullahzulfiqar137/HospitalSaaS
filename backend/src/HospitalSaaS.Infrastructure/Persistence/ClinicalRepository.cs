using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.MedicalRecords;
using HospitalSaaS.Domain.Prescriptions;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class ClinicalRepository(HospitalDbContext db) : IClinicalRepository
{
    public Task AddMedicalRecordAsync(MedicalRecord x, CancellationToken ct) =>
        db.MedicalRecords.AddAsync(x, ct).AsTask();

    public Task<MedicalRecord?> GetMedicalRecordAsync(
        Guid id,
        bool tracking,
        CancellationToken ct
    ) =>
        (tracking ? db.MedicalRecords : db.MedicalRecords.AsNoTracking())
            .Include(x => x.Diagnoses)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

    public async Task<PagedResult<MedicalRecord>> SearchMedicalRecordsAsync(
        Guid? patientId,
        int page,
        int size,
        CancellationToken ct
    )
    {
        try
        {
            IQueryable<MedicalRecord> q = db.MedicalRecords.AsNoTracking().Include(x => x.Diagnoses);
            if (patientId.HasValue)
                q = q.Where(x => x.PatientId == patientId);
            var count = await q.CountAsync(ct);
            var items = await q.OrderByDescending(x => x.CreatedAtUtc)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(ct);
            return new(items, page, size, count);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return new([], page, size, 0);
        }
    }

    public void RemoveMedicalRecord(MedicalRecord x) => db.MedicalRecords.Remove(x);

    public Task AddPrescriptionAsync(Prescription x, CancellationToken ct) =>
        db.Prescriptions.AddAsync(x, ct).AsTask();

    public Task<Prescription?> GetPrescriptionAsync(Guid id, CancellationToken ct) =>
        db
            .Prescriptions.AsNoTracking()
            .Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.Id == id, ct);

    public async Task<PagedResult<Prescription>> SearchPrescriptionsAsync(
        Guid? patientId,
        int page,
        int size,
        CancellationToken ct
    )
    {
        try
        {
            IQueryable<Prescription> q = db.Prescriptions.AsNoTracking().Include(x => x.Items);
            if (patientId.HasValue)
                q = q.Where(x => x.PatientId == patientId);
            var count = await q.CountAsync(ct);
            var items = await q.OrderByDescending(x => x.DateUtc)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(ct);
            return new(items, page, size, count);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return new([], page, size, 0);
        }
    }

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
