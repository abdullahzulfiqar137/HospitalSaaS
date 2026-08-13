using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Domain.Departments;

namespace HospitalSaaS.Application.Departments;

public sealed class CreateDepartmentCommandHandler(
    IDepartmentRepository repository,
    ITenantContext tenant
) : IRequestHandler<CreateDepartmentCommand, Result<DepartmentDto>>
{
    public async Task<Result<DepartmentDto>> Handle(
        CreateDepartmentCommand command,
        CancellationToken cancellationToken
    )
    {
        if (await repository.CodeExistsAsync(command.Code, null, cancellationToken))
            return Result<DepartmentDto>.Failure(
                new(
                    "Departments.CodeConflict",
                    "Department code already exists.",
                    ErrorType.Conflict
                )
            );
        var department = new Department(
            tenant.TenantId,
            command.Name,
            command.Code,
            command.Description
        );
        await repository.AddAsync(department, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return Result<DepartmentDto>.Success(DepartmentDto.From(department));
    }
}
