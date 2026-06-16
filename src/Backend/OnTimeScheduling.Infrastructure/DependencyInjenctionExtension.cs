using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Auth;
using OnTimeScheduling.Application.Repositories.Clients;
using OnTimeScheduling.Application.Repositories.Companies;
using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Repositories.Reports;
using OnTimeScheduling.Application.Repositories.ScheduleBlocks;
using OnTimeScheduling.Application.Repositories.Schedules;
using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Password;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Infrastructure.Persistence.DataAccess;
using OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;
using OnTimeScheduling.Infrastructure.Security.Password;
using OnTimeScheduling.Infrastructure.Security.Tenant;
using OnTimeScheduling.Infrastructure.Security.Tokens;

namespace OnTimeScheduling.Infrastructure;

public static class DependencyInjenctionExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration) 
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("PostgreSQL"))
        );

        services.AddScoped<IUserRepository, UserRepository>();

        //Company Repository
        services.AddScoped<CompanyRepository>();
        services.AddScoped<ICompanyWriteOnlyRepository>(sp => sp.GetRequiredService<CompanyRepository>());
        services.AddScoped<ICompanyReadOnlyRepository>(sp => sp.GetRequiredService<CompanyRepository>());

        //Client Repository
        services.AddScoped<ClientRepository>();
        services.AddScoped<IClientWriteOnlyRepository>(sp => sp.GetRequiredService<ClientRepository>());
        services.AddScoped<IClientReadOnlyRepository>(sp => sp.GetRequiredService<ClientRepository>());

        //Company Repository
        services.AddScoped<LocationRepository>();
        services.AddScoped<ILocationWriteOnlyRepository>(sp => sp.GetRequiredService<LocationRepository>());
        services.AddScoped<ILocationReadOnlyRepository>(sp => sp.GetRequiredService<LocationRepository>());

        //Service Repository
        services.AddScoped<ServiceRepository>();
        services.AddScoped<IServiceWriteOnlyRepository>(sp => sp.GetRequiredService<ServiceRepository>());
        services.AddScoped<IServiceReadOnlyRepository>(sp => sp.GetRequiredService<ServiceRepository>());

        //Link Professional Service Repository
        services.AddScoped<ProfessionalServiceRepository>();
        services.AddScoped<IProfessionalServiceWriteOnlyRepository>(sp => sp.GetRequiredService<ProfessionalServiceRepository>());
        services.AddScoped<IProfessionalServiceReadOnlyRepository>(sp => sp.GetRequiredService<ProfessionalServiceRepository>());

        //Professional Schedule Repository
        services.AddScoped<ProfessionalScheduleRepository>();
        services.AddScoped<IProfessionalScheduleReadOnlyRepository>(sp => sp.GetRequiredService<ProfessionalScheduleRepository>());
        services.AddScoped<IProfessionalScheduleWriteOnlyRepository>(sp => sp.GetRequiredService<ProfessionalScheduleRepository>());

        //Appointment Repository
        services.AddScoped<AppointmentRepository>();
        services.AddScoped<IAppointmentReadOnlyRepository>(sp => sp.GetRequiredService<AppointmentRepository>());
        services.AddScoped<IAppointmentWriteOnlyRepository>(sp => sp.GetRequiredService<AppointmentRepository>());

        //Schedule Block Repository
        services.AddScoped<ScheduleBlockRepository>();
        services.AddScoped<IScheduleBlockReadOnlyRepository>(sp => sp.GetRequiredService<ScheduleBlockRepository>());
        services.AddScoped<IScheduleBlockWriteOnlyRepository>(sp => sp.GetRequiredService<ScheduleBlockRepository>());

        services.AddScoped<IReportsReadOnlyRepository, ReportsRepository>();

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPasswordHashService, PasswordHashService>();

        services.AddScoped<ITenantProvider, TenantProvider>();

        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddScoped<IRefreshTokenSettings>(sp =>
        {
            var expirationDays = configuration.GetValue<uint>("Settings:Jwt:RefreshExpirationDays");
            return new RefreshTokenSettings(expirationDays);
        });


        services.AddHttpContextAccessor();

        services.AddScoped<ILoggedUser, LoggedUser>();

        services.AddScoped<IAccessTokenGenerator>(option =>
        {

            var signingKey = configuration.GetValue<string>("Settings:Jwt:SigningKey");
            var expirationTimeMinutes = configuration.GetValue<uint>("Settings:Jwt:ExpirationTimeMinutes");

            return new JwtTokenGenerator(signingKey!, expirationTimeMinutes);
        });

    }
}
