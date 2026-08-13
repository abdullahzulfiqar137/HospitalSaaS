using HospitalSaaS.Application.Appointments.Commands.UpdateAppointment;
using HospitalSaaS.Application.Clinical;
using HospitalSaaS.Application.Inventory;
using HospitalSaaS.Application.Laboratory;
using HospitalSaaS.Application.Radiology;

namespace HospitalSaaS.Application.Tests;

public sealed class CompletedModuleValidatorTests
{
    [Fact]
    public async Task Appointment_update_rejects_non_utc_and_invalid_duration()
    {
        var command = new UpdateAppointmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now.AddDays(1),
            1,
            "",
            "",
            null
        );
        var result = await new UpdateAppointmentCommandValidator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            x => x.PropertyName == nameof(command.AppointmentDateTimeUtc)
        );
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.DurationMinutes));
    }

    [Fact]
    public async Task Medical_record_rejects_multiple_primary_diagnoses()
    {
        var command = new CreateMedicalRecordCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            "Complaint",
            null,
            null,
            null,
            null,
            null,
            [new("A01", "One", true), new("B01", "Two", true)]
        );
        var result = await new CreateMedicalRecordCommandValidator().ValidateAsync(command);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Catalog_validators_reject_negative_prices()
    {
        var labCommand = new CreateLabTestCommand("Test", "T1", "Category", -1, null, null);
        var radiologyCommand = new CreateRadiologyTestCommand("R1", "Test", "XR", -1);
        var lab = await new CreateLabTestCommandValidator().ValidateAsync(labCommand);
        var radiology = await new CreateRadiologyTestCommandValidator().ValidateAsync(
            radiologyCommand
        );

        Assert.False(lab.IsValid);
        Assert.False(radiology.IsValid);
    }

    [Fact]
    public async Task Purchase_order_rejects_empty_items()
    {
        var command = new CreatePurchaseOrderCommand(Guid.NewGuid(), null, null, []);
        var result = await new CreatePurchaseOrderCommandValidator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.Items));
    }
}
