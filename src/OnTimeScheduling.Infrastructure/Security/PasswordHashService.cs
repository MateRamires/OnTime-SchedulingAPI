using OnTimeScheduling.Application.Security;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Infrastructure.Security
{
    public class PasswordHashService : IPasswordHashService
    {
        public string Hash(string password)
        {
            throw new NotImplementedException();
        }

        public PasswordVerifyResult Verify(string passwordHash, string providedPassword)
        {
            throw new NotImplementedException();
        }
    }
}
