using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.Security.Password;

public interface IPasswordHashService
{
    string Hash(string password);
    PasswordVerifyResult Verify(string passwordHash, string providedPassword);
}
