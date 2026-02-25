using Microsoft.AspNetCore.Identity;
using OnTimeScheduling.Application.Security.Password;
using OnTimeScheduling.Domain.Entities.User;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Infrastructure.Security.Password;

public class PasswordHashService : IPasswordHashService
{
    private readonly IPasswordHasher<User> _hasher = new PasswordHasher<User>();
    private static readonly User _dummy = CreateDummy();
    public string Hash(string password)
    {
        return _hasher.HashPassword(_dummy, password);
    }

    public PasswordVerifyResult Verify(string passwordHash, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(_dummy, passwordHash, providedPassword);

        if (result == PasswordVerificationResult.Success)
            return PasswordVerifyResult.Success;

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
            return PasswordVerifyResult.SuccessRehashNeeded;

        return PasswordVerifyResult.Failed;
    }
    private static User CreateDummy() => new User(companyId: null, name: "dummy", email: "dummy@local", passwordHash: "x", role: UserRole.SUPER_ADMIN);
}