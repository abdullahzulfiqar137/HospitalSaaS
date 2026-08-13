using FluentValidation;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;
using HospitalSaaS.Application.Common.Models;
using HospitalSaaS.Application.Common.Pagination;
using HospitalSaaS.Domain.MedicalRecords;

namespace HospitalSaaS.Application.Clinical;

public sealed record DiagnosisInput(string Code, string Description, bool IsPrimary);

public sealed record CreateMedicalRecordCommand(
    Guid PatientId,
    Guid DoctorId,
    Guid? AppointmentId,
    string ChiefComplaint,
    string? History,
    string? Examination,
    string? ClinicalNotes,
    string? TreatmentPlan,
    DateOnly? FollowUpDate,
    IReadOnlyList<DiagnosisInput>? Diagnoses = null
) : ICommand<Result<MedicalRecordDto>>;

public sealed class CreateMedicalRecordCommandValidator
    : AbstractValidator<CreateMedicalRecordCommand>
{
    public CreateMedicalRecordCommandValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.ChiefComplaint).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.ClinicalNotes).MaximumLength(8000);
        RuleForEach(x => x.Diagnoses)
            .ChildRules(d =>
            {
                d.RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
                d.RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
            });
        RuleFor(x => x.Diagnoses)
            .Must(x => x is null || x.Count(d => d.IsPrimary) <= 1)
            .WithMessage("Only one primary diagnosis is allowed.");
    }
}

public sealed class CreateMedicalRecordCommandHandler(
    IClinicalRepository repo,
    ITenantContext tenant
) : IRequestHandler<CreateMedicalRecordCommand, Result<MedicalRecordDto>>
{
    public async Task<Result<MedicalRecordDto>> Handle(
        CreateMedicalRecordCommand x,
        CancellationToken ct
    )
    {
        var e = new MedicalRecord(
            tenant.TenantId,
            x.PatientId,
            x.DoctorId,
            x.AppointmentId,
            x.ChiefComplaint,
            x.History,
            x.Examination,
            x.ClinicalNotes,
            x.TreatmentPlan,
            x.FollowUpDate
        );
        foreach (var diagnosis in x.Diagnoses ?? [])
            e.AddDiagnosis(diagnosis.Code, diagnosis.Description, diagnosis.IsPrimary);
        await repo.AddMedicalRecordAsync(e, ct);
        await repo.SaveChangesAsync(ct);
        return Result<MedicalRecordDto>.Success(MedicalRecordDto.From(e));
    }
}

public sealed record AddDiagnosisCommand(
    Guid MedicalRecordId,
    string Code,
    string Description,
    bool IsPrimary
) : ICommand<Result<MedicalRecordDto>>;

public sealed class AddDiagnosisCommandValidator : AbstractValidator<AddDiagnosisCommand>
{
    public AddDiagnosisCommandValidator()
    {
        RuleFor(x => x.MedicalRecordId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}

public sealed class AddDiagnosisCommandHandler(IClinicalRepository repository)
    : IRequestHandler<AddDiagnosisCommand, Result<MedicalRecordDto>>
{
    public async Task<Result<MedicalRecordDto>> Handle(
        AddDiagnosisCommand command,
        CancellationToken cancellationToken
    )
    {
        var record = await repository.GetMedicalRecordAsync(
            command.MedicalRecordId,
            true,
            cancellationToken
        );
        if (record is null)
            return Result<MedicalRecordDto>.Failure(
                new("MedicalRecords.NotFound", "Medical record was not found.", ErrorType.NotFound)
            );
        if (record.Diagnoses.Any(x => x.Code == command.Code.Trim().ToUpperInvariant()))
            return Result<MedicalRecordDto>.Failure(
                new("Diagnoses.Duplicate", "Diagnosis code already exists.", ErrorType.Conflict)
            );
        if (command.IsPrimary && record.Diagnoses.Any(x => x.IsPrimary))
            return Result<MedicalRecordDto>.Failure(
                new(
                    "Diagnoses.PrimaryConflict",
                    "The record already has a primary diagnosis.",
                    ErrorType.Conflict
                )
            );
        record.AddDiagnosis(command.Code, command.Description, command.IsPrimary);
        await repository.SaveChangesAsync(cancellationToken);
        return Result<MedicalRecordDto>.Success(MedicalRecordDto.From(record));
    }
}

public sealed record GetMedicalRecordQuery(Guid Id) : IQuery<Result<MedicalRecordDto>>;

public sealed class GetMedicalRecordQueryHandler(IClinicalRepository repo)
    : IRequestHandler<GetMedicalRecordQuery, Result<MedicalRecordDto>>
{
    public async Task<Result<MedicalRecordDto>> Handle(
        GetMedicalRecordQuery x,
        CancellationToken ct
    )
    {
        var e = await repo.GetMedicalRecordAsync(x.Id, false, ct);
        return e is null
            ? Result<MedicalRecordDto>.Failure(
                new("MedicalRecords.NotFound", "Medical record was not found.", ErrorType.NotFound)
            )
            : Result<MedicalRecordDto>.Success(MedicalRecordDto.From(e));
    }
}

public sealed record SearchMedicalRecordsQuery(Guid? PatientId, int PageNumber, int PageSize)
    : IQuery<PagedResult<MedicalRecordDto>>;

public sealed class SearchMedicalRecordsQueryValidator
    : AbstractValidator<SearchMedicalRecordsQuery>
{
    public SearchMedicalRecordsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class SearchMedicalRecordsQueryHandler(IClinicalRepository repo)
    : IRequestHandler<SearchMedicalRecordsQuery, PagedResult<MedicalRecordDto>>
{
    public async Task<PagedResult<MedicalRecordDto>> Handle(
        SearchMedicalRecordsQuery x,
        CancellationToken ct
    )
    {
        var p = await repo.SearchMedicalRecordsAsync(x.PatientId, x.PageNumber, x.PageSize, ct);
        return new(
            p.Items.Select(MedicalRecordDto.From).ToList(),
            p.PageNumber,
            p.PageSize,
            p.TotalCount
        );
    }
}

public sealed record DeleteMedicalRecordCommand(Guid Id) : ICommand<Result>;

public sealed class DeleteMedicalRecordCommandHandler(IClinicalRepository repo)
    : IRequestHandler<DeleteMedicalRecordCommand, Result>
{
    public async Task<Result> Handle(DeleteMedicalRecordCommand x, CancellationToken ct)
    {
        var e = await repo.GetMedicalRecordAsync(x.Id, true, ct);
        if (e is null)
            return Result.Failure(
                new("MedicalRecords.NotFound", "Medical record was not found.", ErrorType.NotFound)
            );
        repo.RemoveMedicalRecord(e);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
