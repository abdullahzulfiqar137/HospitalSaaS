using HospitalSaaS.Domain.Patients;
using HospitalSaaS.Infrastructure.Persistence;
using HospitalSaaS.Infrastructure.Tenancy;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HospitalSaaS.IntegrationTests;

public sealed class TenantIsolationTests
{
    private static string ConnectionString =>
        Environment.GetEnvironmentVariable("TEST_SQL_CONNECTION")
        ?? "Server=DESKTOP-O8LF2FG;Database=HospitalSaaSDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

    [Fact]
    public async Task Global_filter_prevents_cross_tenant_patient_reads()
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<HospitalDbContext>()
            .UseSqlServer(connection)
            .Options;
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();
        var seedingContext = new TenantContext();
        await using var seedDb = new HospitalDbContext(options, seedingContext);
        await using var transaction = await seedDb.Database.BeginTransactionAsync();

        using (seedingContext.BeginSuperAdminBypass(true))
        {
            seedDb.Patients.Add(CreatePatient(tenantA, "PAT-2099-900001"));
            seedDb.Patients.Add(CreatePatient(tenantB, "PAT-2099-900002"));
            await seedDb.SaveChangesAsync();
        }

        var requestTenant = new TenantContext();
        requestTenant.SetTenant(tenantA);
        await using var requestDb = new HospitalDbContext(options, requestTenant);
        await requestDb.Database.UseTransactionAsync(transaction.GetDbTransaction());
        var visibleNumbers = await requestDb.Patients.Select(x => x.PatientNumber).ToListAsync();

        Assert.Contains("PAT-2099-900001", visibleNumbers);
        Assert.DoesNotContain("PAT-2099-900002", visibleNumbers);
        await transaction.RollbackAsync();
    }

    private static Patient CreatePatient(Guid tenantId, string number) =>
        new(
            tenantId,
            number,
            "Test",
            null,
            "Patient",
            new DateOnly(1990, 1, 1),
            "Other",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );
}

public sealed class TenantBypassTests
{
    [Fact]
    public void Unauthorized_bypass_is_rejected() =>
        Assert.Throws<UnauthorizedAccessException>(() =>
            new TenantContext().BeginSuperAdminBypass(false)
        );
}
