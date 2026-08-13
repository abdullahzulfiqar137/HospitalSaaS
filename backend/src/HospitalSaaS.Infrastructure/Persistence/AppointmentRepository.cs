using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.Appointments;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.Infrastructure.Persistence;

public sealed class AppointmentRepository(HospitalDbContext dbContext) : IAppointmentRepository
{
    public Task AddAsync(Appointment appointment, CancellationToken cancellationToken) =>
        dbContext.Appointments.AddAsync(appointment, cancellationToken).AsTask();

    public Task<Appointment?> GetAsync(Guid id, bool tracking, CancellationToken cancellationToken)
    {
        var query = tracking ? dbContext.Appointments : dbContext.Appointments.AsNoTracking();
        return query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> HasDoctorConflictAsync(
        Guid doctorId,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        Guid? excludingAppointmentId,
        CancellationToken cancellationToken
    ) =>
        dbContext.Appointments.AnyAsync(
            x =>
                x.DoctorId == doctorId
                && (!excludingAppointmentId.HasValue || x.Id != excludingAppointmentId.Value)
                && x.Status != AppointmentStatus.Cancelled
                && x.Status != AppointmentStatus.NoShow
                && x.AppointmentDateTimeUtc < endsAtUtc
                && x.AppointmentDateTimeUtc.AddMinutes(x.DurationMinutes) > startsAtUtc,
            cancellationToken
        );

    public async Task<PagedResult<Appointment>> SearchAsync(
        DateTime? date,
        Guid? doctorId,
        Guid? patientId,
        AppointmentStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        var query = dbContext.Appointments.AsNoTracking();
        if (date.HasValue)
        {
            var start = date.Value.Date;
            query = query.Where(x =>
                x.AppointmentDateTimeUtc >= start && x.AppointmentDateTimeUtc < start.AddDays(1)
            );
        }
        if (doctorId.HasValue)
            query = query.Where(x => x.DoctorId == doctorId.Value);
        if (patientId.HasValue)
            query = query.Where(x => x.PatientId == patientId.Value);
        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.AppointmentDateTimeUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return new PagedResult<Appointment>(items, pageNumber, pageSize, totalCount);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
