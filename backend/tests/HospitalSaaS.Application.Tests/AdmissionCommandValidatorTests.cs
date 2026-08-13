using HospitalSaaS.Application.Admissions;

namespace HospitalSaaS.Application.Tests;

public sealed class AdmissionCommandValidatorTests
{
    [Fact]
    public async Task AdmitPatient_Requires_All_Relationships_And_Clinical_Fields()
    {
        var command = new AdmitPatientCommand(
            Guid.Empty,
            Guid.Empty,
            "",
            "",
            Guid.Empty,
            Guid.Empty,
            Guid.Empty
        );

        var result = await new AdmitPatientCommandValidator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.PatientId));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.DoctorId));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.BedId));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.Reason));
    }

    [Fact]
    public async Task Transfer_Requires_Admission_And_Target_Location()
    {
        var command = new TransferAdmissionCommand(Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty);

        var result = await new TransferAdmissionCommandValidator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Equal(4, result.Errors.Count);
    }
}
