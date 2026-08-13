using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Doctors.Models;

namespace HospitalSaaS.Application.Doctors.Queries.GetDoctorById;

public sealed class GetDoctorByIdQueryHandler(IDoctorRepository repository)
    : IRequestHandler<GetDoctorByIdQuery, Result<DoctorDto>>
{
    public async Task<Result<DoctorDto>> Handle(
        GetDoctorByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var doctor = await repository.GetAsync(query.Id, false, cancellationToken);
        return doctor is null
            ? Result<DoctorDto>.Failure(
                new("Doctors.NotFound", "Doctor was not found.", ErrorType.NotFound)
            )
            : Result<DoctorDto>.Success(DoctorDto.From(doctor));
    }
}
