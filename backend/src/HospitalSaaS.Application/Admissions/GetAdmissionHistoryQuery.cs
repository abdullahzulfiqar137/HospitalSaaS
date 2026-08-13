using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;

namespace HospitalSaaS.Application.Admissions;

public sealed record GetAdmissionHistoryQuery(Guid AdmissionId)
    : IQuery<IReadOnlyList<AdmissionHistoryDto>>;

public sealed class GetAdmissionHistoryQueryHandler(IAdmissionRepository repository)
    : IRequestHandler<GetAdmissionHistoryQuery, IReadOnlyList<AdmissionHistoryDto>>
{
    public async Task<IReadOnlyList<AdmissionHistoryDto>> Handle(
        GetAdmissionHistoryQuery query,
        CancellationToken cancellationToken
    ) =>
        (await repository.GetHistoryAsync(query.AdmissionId, cancellationToken))
            .Select(AdmissionHistoryDto.From)
            .ToList();
}
