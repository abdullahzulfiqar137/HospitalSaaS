using HospitalSaaS.Domain.Admissions;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IAdmissionRepository
{
    Task AddWardAsync(Ward ward, CancellationToken cancellationToken);
    Task AddRoomAsync(Room room, CancellationToken cancellationToken);
    Task AddBedAsync(Bed bed, CancellationToken cancellationToken);
    Task AddAdmissionAsync(Admission admission, CancellationToken cancellationToken);
    Task AddHistoryAsync(AdmissionHistory history, CancellationToken cancellationToken);
    Task<Ward?> GetWardAsync(Guid id, CancellationToken cancellationToken);
    Task<Room?> GetRoomAsync(Guid id, CancellationToken cancellationToken);
    Task<Bed?> GetBedAsync(Guid id, bool tracking, CancellationToken cancellationToken);
    Task<Admission?> GetAdmissionAsync(Guid id, bool tracking, CancellationToken cancellationToken);
    Task<IReadOnlyList<AdmissionHistory>> GetHistoryAsync(
        Guid admissionId,
        CancellationToken cancellationToken
    );
    Task<bool> WardCodeExistsAsync(string code, CancellationToken cancellationToken);
    Task<bool> RoomNumberExistsAsync(
        Guid wardId,
        string number,
        CancellationToken cancellationToken
    );
    Task<bool> BedNumberExistsAsync(
        Guid roomId,
        string number,
        CancellationToken cancellationToken
    );
    Task<int> CountAdmissionsInYearAsync(int year, CancellationToken cancellationToken);
    Task<bool> AdmissionNumberExistsAsync(string number, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
