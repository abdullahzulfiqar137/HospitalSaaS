using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;

namespace HospitalSaaS.Application.Departments;

public sealed record DeleteDepartmentCommand(Guid Id) : ICommand<Result>;

public sealed class DeleteDepartmentCommandHandler(IDepartmentRepository repository)
    : IRequestHandler<DeleteDepartmentCommand, Result>
{
    public async Task<Result> Handle(
        DeleteDepartmentCommand command,
        CancellationToken cancellationToken
    )
    {
        var department = await repository.GetAsync(command.Id, true, cancellationToken);
        if (department is null)
            return Result.Failure(
                new("Departments.NotFound", "Department was not found.", ErrorType.NotFound)
            );
        repository.Remove(department);
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
