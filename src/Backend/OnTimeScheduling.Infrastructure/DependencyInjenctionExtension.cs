using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnTimeScheduling.Application.Repositories.Companies;
using OnTimeScheduling.Application.Repositories.Locations;
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

        //Company Repository
        services.AddScoped<LocationRepository>();
        services.AddScoped<ILocationWriteOnlyRepository>(sp => sp.GetRequiredService<LocationRepository>());
        services.AddScoped<ILocationReadOnlyRepository>(sp => sp.GetRequiredService<LocationRepository>());

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPasswordHashService, PasswordHashService>();

        services.AddScoped<ITenantProvider, TenantProvider>();

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
