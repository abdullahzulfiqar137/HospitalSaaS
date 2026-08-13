using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Domain.Admissions;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class AdmissionRepository(HospitalDbContext db) : IAdmissionRepository
{
    public Task AddWardAsync(Ward x, CancellationToken ct) => db.Wards.AddAsync(x, ct).AsTask();

    public Task AddRoomAsync(Room x, CancellationToken ct) => db.Rooms.AddAsync(x, ct).AsTask();

    public Task AddBedAsync(Bed x, CancellationToken ct) => db.Beds.AddAsync(x, ct).AsTask();

    public Task AddAdmissionAsync(Admission x, CancellationToken ct) =>
        db.Admissions.AddAsync(x, ct).AsTask();

    public Task AddHistoryAsync(AdmissionHistory x, CancellationToken ct) =>
        db.AdmissionHistories.AddAsync(x, ct).AsTask();

    public Task<Ward?> GetWardAsync(Guid id, CancellationToken ct) =>
        db.Wards.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);

    public Task<Room?> GetRoomAsync(Guid id, CancellationToken ct) =>
        db.Rooms.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);

    public Task<Bed?> GetBedAsync(Guid id, bool tracking, CancellationToken ct) =>
        (tracking ? db.Beds : db.Beds.AsNoTracking()).SingleOrDefaultAsync(x => x.Id == id, ct);

    public Task<Admission?> GetAdmissionAsync(Guid id, bool tracking, CancellationToken ct) =>
        (tracking ? db.Admissions : db.Admissions.AsNoTracking()).SingleOrDefaultAsync(
            x => x.Id == id,
            ct
        );

    public async Task<IReadOnlyList<AdmissionHistory>> GetHistoryAsync(
        Guid admissionId,
        CancellationToken ct
    ) =>
        await db
            .AdmissionHistories.AsNoTracking()
            .Where(x => x.AdmissionId == admissionId)
            .OrderBy(x => x.TransferredAtUtc)
            .ToListAsync(ct);

    public Task<bool> WardCodeExistsAsync(string code, CancellationToken ct) =>
        db.Wards.AnyAsync(x => x.Code == code.Trim().ToUpper(), ct);

    public Task<bool> RoomNumberExistsAsync(Guid wardId, string number, CancellationToken ct) =>
        db.Rooms.AnyAsync(x => x.WardId == wardId && x.Number == number.Trim(), ct);

    public Task<bool> BedNumberExistsAsync(Guid roomId, string number, CancellationToken ct) =>
        db.Beds.AnyAsync(x => x.RoomId == roomId && x.Number == number.Trim(), ct);

    public Task<int> CountAdmissionsInYearAsync(int year, CancellationToken ct) =>
        db.Admissions.CountAsync(x => x.AdmissionDateUtc.Year == year, ct);

    public Task<bool> AdmissionNumberExistsAsync(string number, CancellationToken ct) =>
        db.Admissions.AnyAsync(x => x.AdmissionNumber == number, ct);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
