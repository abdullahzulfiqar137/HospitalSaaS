using HospitalSaaS.Application.Patients.Commands.CreatePatient;

namespace HospitalSaaS.Application.Tests;

public sealed class CreatePatientValidatorTests
{
    [Fact]
    public async Task Invalid_patient_is_rejected()
    {
        var validator = new CreatePatientCommandValidator();
        var command = new CreatePatientCommand(
            "",
            null,
            "",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            "",
            null,
            "not-email",
            null,
            null,
            null,
            null,
            null,
            null
        );
        var result = await validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.DateOfBirth));
    }
}
