using HospitalSaaS.Api.Extensions;
using HospitalSaaS.Application;
using HospitalSaaS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder
    .Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApiServices(builder.Configuration);
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    await scope
        .ServiceProvider.GetRequiredService<HospitalSaaS.Infrastructure.Persistence.Seed.DatabaseSeeder>()
        .SeedAsync();
}
app.UseApiPipeline();
app.Run();

public partial class Program;
