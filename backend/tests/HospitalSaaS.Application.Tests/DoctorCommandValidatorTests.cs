using HospitalSaaS.Application.Doctors.Commands.CreateDoctor;
using HospitalSaaS.Application.Doctors.Queries.SearchDoctors;

namespace HospitalSaaS.Application.Tests;

public sealed class DoctorCommandValidatorTests
{
    [Fact]
    public async Task CreateDoctor_Rejects_Invalid_Required_Fields_And_Fee()
    {
        var command = new CreateDoctorCommand(
            "",
            "",
            "",
            "",
            Guid.Empty,
            "",
            null,
            "not-an-email",
            -1
        );

        var result = await new CreateDoctorCommandValidator().ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.EmployeeNumber));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.DepartmentId));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.Email));
        Assert.Contains(result.Errors, x => x.PropertyName == nameof(command.ConsultationFee));
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public async Task SearchDoctors_Rejects_Invalid_Paging(int pageNumber, int pageSize)
    {
        var result = await new SearchDoctorsQueryValidator().ValidateAsync(
            new SearchDoctorsQuery(null, pageNumber, pageSize)
        );

        Assert.False(result.IsValid);
    }
}
