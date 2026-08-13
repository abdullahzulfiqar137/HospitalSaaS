using FluentValidation;
using HospitalSaaS.Contracts.Clinical;
using HospitalSaaS.Contracts.Departments;
using HospitalSaaS.Contracts.Support;

namespace HospitalSaaS.Api.Filters;

public sealed class CreateMedicalRecordRequestValidator
    : AbstractValidator<CreateMedicalRecordRequest>
{
    public CreateMedicalRecordRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.ChiefComplaint).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.FollowUpDate)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .When(x => x.FollowUpDate.HasValue);
    }
}

public sealed class CreatePrescriptionRequestValidator
    : AbstractValidator<CreatePrescriptionRequest>
{
    public CreatePrescriptionRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.MedicineId).NotEmpty();
                item.RuleFor(x => x.Dose).NotEmpty();
                item.RuleFor(x => x.Frequency).NotEmpty();
                item.RuleFor(x => x.Duration).NotEmpty();
                item.RuleFor(x => x.Route).NotEmpty();
            });
    }
}

public sealed class CreateRadiologyOrderRequestValidator
    : AbstractValidator<CreateRadiologyOrderRequest>
{
    public CreateRadiologyOrderRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.Procedure).NotEmpty().MaximumLength(500);
    }
}

public sealed class CreateSupplierRequestValidator : AbstractValidator<CreateSupplierRequest>
{
    public CreateSupplierRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}

public sealed class CreateInsurancePolicyRequestValidator
    : AbstractValidator<CreateInsurancePolicyRequest>
{
    public CreateInsurancePolicyRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.Provider).NotEmpty();
        RuleFor(x => x.PolicyNumber).NotEmpty();
        RuleFor(x => x.ValidTo).GreaterThanOrEqualTo(x => x.ValidFrom);
    }
}

public sealed class CreateStaffRequestValidator : AbstractValidator<CreateStaffRequest>
{
    public CreateStaffRequestValidator()
    {
        RuleFor(x => x.EmployeeNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.JobTitle).NotEmpty().MaximumLength(150);
    }
}

public sealed class CreateSubscriptionRequestValidator
    : AbstractValidator<CreateSubscriptionRequest>
{
    public CreateSubscriptionRequestValidator()
    {
        RuleFor(x => x.Plan).NotEmpty().MaximumLength(100);
        RuleFor(x => x.StartsAtUtc).NotEmpty();
        RuleFor(x => x.EndsAtUtc).GreaterThan(x => x.StartsAtUtc);
    }
}

public sealed class UpdateDepartmentRequestValidator : AbstractValidator<UpdateDepartmentRequest>
{
    public UpdateDepartmentRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
    }
}
