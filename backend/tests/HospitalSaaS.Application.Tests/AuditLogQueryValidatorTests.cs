using HospitalSaaS.Application.AuditLogs;

namespace HospitalSaaS.Application.Tests;

public sealed class AuditLogQueryValidatorTests
{
    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public async Task Search_Rejects_Invalid_Pagination(int pageNumber, int pageSize)
    {
        var query = new SearchAuditLogsQuery(null, null, pageNumber, pageSize);
        var result = await new SearchAuditLogsQueryValidator().ValidateAsync(query);
        Assert.False(result.IsValid);
    }
}
