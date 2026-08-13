using HospitalSaaS.Domain.Laboratory;
using HospitalSaaS.Infrastructure.Persistence;
using HospitalSaaS.Infrastructure.Tenancy;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HospitalSaaS.IntegrationTests;

public sealed class CompletedModulePersistenceTests
{
    private const string ConnectionString =
        "Server=DESKTOP-O8LF2FG;Database=HospitalSaaSDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

    [Fact]
    public async Task Lab_catalog_is_persisted_and_tenant_isolated()
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<HospitalDbContext>()
            .UseSqlServer(connection)
            .Options;
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();
        var seedingTenant = new TenantContext();
        await using var seedDb = new HospitalDbContext(options, seedingTenant);
        await using var transaction = await seedDb.Database.BeginTransactionAsync();

        using (seedingTenant.BeginSuperAdminBypass(true))
        {
            seedDb.LabTests.Add(
                new LabTest(tenantA, "CBC A", "CBC-A", "Hematology", 10, null, null)
            );
            seedDb.LabTests.Add(
                new LabTest(tenantB, "CBC B", "CBC-B", "Hematology", 10, null, null)
            );
            await seedDb.SaveChangesAsync();
        }

        var requestTenant = new TenantContext();
        requestTenant.SetTenant(tenantA);
        await using var requestDb = new HospitalDbContext(options, requestTenant);
        await requestDb.Database.UseTransactionAsync(transaction.GetDbTransaction());
        var visibleCodes = await requestDb.LabTests.Select(x => x.Code).ToListAsync();

        Assert.Contains("CBC-A", visibleCodes);
        Assert.DoesNotContain("CBC-B", visibleCodes);
        await transaction.RollbackAsync();
    }
}
