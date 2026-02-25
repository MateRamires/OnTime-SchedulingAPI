using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Security.Password;
using OnTimeScheduling.Domain.Entities.User;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess;

public class DbInitializer
{
    public static async Task Seed(AppDbContext context, IPasswordHashService passwordHasher)
    {
        bool hasSuperAdmin = await context.Users
            .AnyAsync(u => u.Role == UserRole.SUPER_ADMIN);

        if (!hasSuperAdmin)
        {
            var passwordHash = passwordHasher.Hash("123456");

            var rootUser = new User(
                companyId: null,
                name: "Root",
                email: "matheus@gmail.com",
                passwordHash: passwordHash,
                role: UserRole.SUPER_ADMIN
            );

            context.Users.Add(rootUser);
            await context.SaveChangesAsync();
        }
    }
}
