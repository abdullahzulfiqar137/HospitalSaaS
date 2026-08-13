using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.MedicalRecords;
using HospitalSaaS.Domain.Prescriptions;

namespace HospitalSaaS.Application.Common.Interfaces;

public interface IClinicalRepository
{
    Task AddMedicalRecordAsync(MedicalRecord record, CancellationToken ct);
    Task<MedicalRecord?> GetMedicalRecordAsync(Guid id, bool tracking, CancellationToken ct);
    Task<PagedResult<MedicalRecord>> SearchMedicalRecordsAsync(
        Guid? patientId,
        int page,
        int size,
        CancellationToken ct
    );
    void RemoveMedicalRecord(MedicalRecord record);
    Task AddPrescriptionAsync(Prescription prescription, CancellationToken ct);
    Task<Prescription?> GetPrescriptionAsync(Guid id, CancellationToken ct);
    Task<PagedResult<Prescription>> SearchPrescriptionsAsync(
        Guid? patientId,
        int page,
        int size,
        CancellationToken ct
    );
    Task SaveChangesAsync(CancellationToken ct);
}
