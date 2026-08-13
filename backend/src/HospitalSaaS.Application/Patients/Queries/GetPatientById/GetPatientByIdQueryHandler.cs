using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Patients.Models;

namespace HospitalSaaS.Application.Patients.Queries.GetPatientById;

public sealed class GetPatientByIdQueryHandler(IPatientRepository repository)
    : IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>
{
    public async Task<Result<PatientDto>> Handle(GetPatientByIdQuery query, CancellationToken ct)
    {
        var p = await repository.GetAsync(query.Id, ct);
        return p is null
            ? Result<PatientDto>.Failure(
                new("Patients.NotFound", "Patient was not found.", ErrorType.NotFound)
            )
            : Result<PatientDto>.Success(PatientDto.From(p));
    }
}
