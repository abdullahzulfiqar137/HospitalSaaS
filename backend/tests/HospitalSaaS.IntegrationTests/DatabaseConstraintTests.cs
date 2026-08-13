using HospitalSaaS.Domain.Patients;
using HospitalSaaS.Infrastructure.Persistence;
using HospitalSaaS.Infrastructure.Tenancy;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HospitalSaaS.IntegrationTests;

public sealed class DatabaseConstraintTests
{
    private const string ConnectionString =
        "Server=DESKTOP-O8LF2FG;Database=HospitalSaaSDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";

    [Fact]
    public async Task Patient_number_is_unique_within_tenant()
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<HospitalDbContext>()
            .UseSqlServer(connection)
            .Options;
        var tenant = new TenantContext();
        tenant.SetTenant(Guid.NewGuid());
        await using var db = new HospitalDbContext(options, tenant);
        await using var transaction = await db.Database.BeginTransactionAsync();
        var number = $"PAT-2099-{Random.Shared.Next(100000, 999999)}";
        db.Patients.Add(CreatePatient(tenant.TenantId, number));
        db.Patients.Add(CreatePatient(tenant.TenantId, number));

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        await transaction.RollbackAsync();
    }

    private static Patient CreatePatient(Guid tenantId, string number) =>
        new(
            tenantId,
            number,
            "Constraint",
            null,
            "Test",
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
