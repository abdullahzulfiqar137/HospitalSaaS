using FluentValidation;
using HospitalSaaS.Contracts.Admissions;
using HospitalSaaS.Contracts.Appointments;
using HospitalSaaS.Contracts.Billing;
using HospitalSaaS.Contracts.Doctors;
using HospitalSaaS.Contracts.Inventory;
using HospitalSaaS.Contracts.Laboratory;
using HospitalSaaS.Contracts.Pharmacy;
using HospitalSaaS.Contracts.Users;

namespace HospitalSaaS.Api.Filters;

public sealed class CreateDoctorRequestValidator : AbstractValidator<CreateDoctorRequest>
{
    public CreateDoctorRequestValidator()
    {
        RuleFor(x => x.EmployeeNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DepartmentId).NotEmpty();
        RuleFor(x => x.LicenseNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.ConsultationFee).GreaterThanOrEqualTo(0);
    }
}

public sealed class BookAppointmentRequestValidator : AbstractValidator<BookAppointmentRequest>
{
    public BookAppointmentRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.DepartmentId).NotEmpty();
        RuleFor(x => x.AppointmentDateTimeUtc).GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.DurationMinutes).InclusiveBetween(5, 480);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}

public sealed class AdmitPatientRequestValidator : AbstractValidator<AdmitPatientRequest>
{
    public AdmitPatientRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.WardId).NotEmpty();
        RuleFor(x => x.RoomId).NotEmpty();
        RuleFor(x => x.BedId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty();
    }
}

public sealed class CreateMedicineRequestValidator : AbstractValidator<CreateMedicineRequest>
{
    public CreateMedicineRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.GenericName).NotEmpty();
        RuleFor(x => x.SalePrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MinimumStockLevel).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateLabOrderRequestValidator : AbstractValidator<CreateLabOrderRequest>
{
    public CreateLabOrderRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.Tests).NotEmpty();
        RuleForEach(x => x.Tests)
            .ChildRules(test =>
            {
                test.RuleFor(x => x.Code).NotEmpty();
                test.RuleFor(x => x.Name).NotEmpty();
            });
    }
}

public sealed class CreateInvoiceRequestValidator : AbstractValidator<CreateInvoiceRequest>
{
    public CreateInvoiceRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.Quantity).GreaterThan(0);
                item.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
            });
    }
}

public sealed class CreateInventoryItemRequestValidator
    : AbstractValidator<CreateInventoryItemRequest>
{
    public CreateInventoryItemRequestValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MinimumStock).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).MinimumLength(12);
        RuleFor(x => x.RoleIds).NotEmpty();
    }
}
