using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Departments;

public sealed record GetDepartmentByIdQuery(Guid Id) : IQuery<Result<DepartmentDto>>;

public sealed class GetDepartmentByIdQueryHandler(IDepartmentRepository repository)
    : IRequestHandler<GetDepartmentByIdQuery, Result<DepartmentDto>>
{
    public async Task<Result<DepartmentDto>> Handle(
        GetDepartmentByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var department = await repository.GetAsync(query.Id, false, cancellationToken);
        return department is null
            ? Result<DepartmentDto>.Failure(
                new("Departments.NotFound", "Department was not found.", ErrorType.NotFound)
            )
            : Result<DepartmentDto>.Success(DepartmentDto.From(department));
    }
}
