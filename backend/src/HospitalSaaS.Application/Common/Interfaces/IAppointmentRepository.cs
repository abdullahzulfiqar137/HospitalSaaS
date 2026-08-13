using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Appointments;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IAppointmentRepository
{
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken);
    Task<Appointment?> GetAsync(Guid id, bool tracking, CancellationToken cancellationToken);
    Task<bool> HasDoctorConflictAsync(
        Guid doctorId,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        Guid? excludingAppointmentId,
        CancellationToken cancellationToken
    );
    Task<PagedResult<Appointment>> SearchAsync(
        DateTime? date,
        Guid? doctorId,
        Guid? patientId,
        AppointmentStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
