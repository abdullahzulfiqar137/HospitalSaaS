using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Domain.Users;
using HospitalSaaS.Infrastructure.Authentication;
using HospitalSaaS.Infrastructure.BackgroundJobs;
using HospitalSaaS.Infrastructure.Caching;
using HospitalSaaS.Infrastructure.Email;
using HospitalSaaS.Infrastructure.Persistence;
using HospitalSaaS.Infrastructure.Persistence.Interceptors;
using HospitalSaaS.Infrastructure.Persistence.Seed;
using HospitalSaaS.Infrastructure.Sms;
using HospitalSaaS.Infrastructure.Storage;
using HospitalSaaS.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalSaaS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connection =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing.");

        services.AddScoped<SoftDeleteInterceptor>();
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<DomainEventInterceptor>();
        services.AddScoped<IDomainEventDispatcher, LoggingDomainEventDispatcher>();
        services.AddDbContext<HospitalDbContext>(
            (provider, options) =>
                options
                    .UseSqlServer(connection)
                    .AddInterceptors(
                        provider.GetRequiredService<SoftDeleteInterceptor>(),
                        provider.GetRequiredService<AuditableEntityInterceptor>(),
                        provider.GetRequiredService<DomainEventInterceptor>()
                    )
        );

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.Configure<SmsOptions>(configuration.GetSection(SmsOptions.SectionName));
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IAdmissionRepository, AdmissionRepository>();
        services.AddScoped<IClinicalRepository, ClinicalRepository>();
        services.AddScoped<IRadiologyRepository, RadiologyRepository>();
        services.AddScoped<IPharmacyRepository, PharmacyRepository>();
        services.AddScoped<ILaboratoryRepository, LaboratoryRepository>();
        services.AddScoped<IBillingRepository, BillingRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IInsuranceRepository, InsuranceRepository>();
        services.AddScoped<IStaffRepository, StaffRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IReportingRepository, ReportingRepository>();
        services.AddScoped<IAdministrationRepository, AdministrationRepository>();
        services.AddScoped<IUserPasswordHasher, UserPasswordHasher>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddHttpClient<ISmsService, SmsService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();
        services.AddSingleton<BackgroundJobService>();
        services.AddSingleton<IBackgroundJobService>(provider =>
            provider.GetRequiredService<BackgroundJobService>()
        );
        services.AddHostedService(provider => provider.GetRequiredService<BackgroundJobService>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<DatabaseSeeder>();
        services.AddHealthChecks().AddDbContextCheck<HospitalDbContext>("sql-server");
        return services;
    }
}
