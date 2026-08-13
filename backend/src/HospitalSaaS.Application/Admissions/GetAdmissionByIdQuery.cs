using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Admissions;

public sealed record GetAdmissionByIdQuery(Guid Id) : IQuery<Result<AdmissionDto>>;

public sealed class GetAdmissionByIdQueryHandler(IAdmissionRepository repository)
    : IRequestHandler<GetAdmissionByIdQuery, Result<AdmissionDto>>
{
    public async Task<Result<AdmissionDto>> Handle(
        GetAdmissionByIdQuery query,
        CancellationToken ct
    )
    {
        var admission = await repository.GetAdmissionAsync(query.Id, false, ct);
        return admission is null
            ? Result<AdmissionDto>.Failure(
                new("Admissions.NotFound", "Admission was not found.", ErrorType.NotFound)
            )
            : Result<AdmissionDto>.Success(AdmissionDto.From(admission));
    }
}
