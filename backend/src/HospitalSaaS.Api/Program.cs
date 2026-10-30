using HospitalSaaS.Api.Extensions;
using HospitalSaaS.Application;
using HospitalSaaS.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder
    .Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApiServices(builder.Configuration);
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<HospitalSaaS.Infrastructure.Persistence.HospitalDbContext>();
    await db.Database.MigrateAsync();
    await services
        .GetRequiredService<HospitalSaaS.Infrastructure.Persistence.Seed.DatabaseSeeder>()
        .SeedAsync();
}
app.UseApiPipeline();
app.Run();

public partial class Program;
