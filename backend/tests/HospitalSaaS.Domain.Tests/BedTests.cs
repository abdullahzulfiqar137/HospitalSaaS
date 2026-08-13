using HospitalSaaS.Domain.Admissions;
using HospitalSaaS.Domain.Exceptions;

namespace HospitalSaaS.Domain.Tests;

public sealed class BedTests
{
    [Fact]
    public void Occupied_bed_cannot_be_occupied_again()
    {
        var bed = new Bed(Guid.NewGuid(), Guid.NewGuid(), "A1");
        bed.Occupy();
        Assert.Throws<DomainException>(bed.Occupy);
    }
}
