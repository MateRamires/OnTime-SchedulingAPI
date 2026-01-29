using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Password;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Infrastructure.Persistence.DataAccess;
using OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;
using OnTimeScheduling.Infrastructure.Security;
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

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPasswordHashService, PasswordHashService>();

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
