using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnTimeScheduling.Application.Security.Password;
using OnTimeScheduling.Domain.Entities.User;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess;

public class DbInitializer
{
    public static async Task Seed(AppDbContext context, IPasswordHashService passwordHasher, IConfiguration configuration)
    {
        bool hasSuperAdmin = await context.Users
            .AnyAsync(u => u.Role == UserRole.SUPER_ADMIN);

        if (!hasSuperAdmin)
        {
            var email = configuration.GetValue<string>("Seed:SuperAdmin:Email");
            var password = configuration.GetValue<string>("Seed:SuperAdmin:Password");

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            var passwordHash = passwordHasher.Hash(password);

            var rootUser = new User(
                companyId: null,
                name: "Root",
                email: email,
                passwordHash: passwordHash,
                role: UserRole.SUPER_ADMIN
            );

            context.Users.Add(rootUser);
            await context.SaveChangesAsync();
        }
    }
}
