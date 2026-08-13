using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Departments;

public sealed class UpdateDepartmentCommandHandler(IDepartmentRepository repository)
    : IRequestHandler<UpdateDepartmentCommand, Result<DepartmentDto>>
{
    public async Task<Result<DepartmentDto>> Handle(
        UpdateDepartmentCommand command,
        CancellationToken cancellationToken
    )
    {
        var department = await repository.GetAsync(command.Id, true, cancellationToken);
        if (department is null)
            return NotFound();
        if (await repository.CodeExistsAsync(command.Code, command.Id, cancellationToken))
            return Result<DepartmentDto>.Failure(
                new(
                    "Departments.CodeConflict",
                    "Department code already exists.",
                    ErrorType.Conflict
                )
            );
        department.Update(command.Name, command.Code, command.Description, command.HeadDoctorId);
        await repository.SaveChangesAsync(cancellationToken);
        return Result<DepartmentDto>.Success(DepartmentDto.From(department));
    }

    private static Result<DepartmentDto> NotFound() =>
        Result<DepartmentDto>.Failure(
            new("Departments.NotFound", "Department was not found.", ErrorType.NotFound)
        );
}
