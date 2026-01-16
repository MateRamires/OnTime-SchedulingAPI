using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Domain.Entities.User;

namespace OnTimeScheduling.Infrastructure.Security.Tokens;

public class JwtTokenGenerator : IAcessTokenGenerator
{
    private readonly string _signingKey;
    private readonly uint _expirationTimeMinutes;
    public JwtTokenGenerator(string signingKey, uint expirationTimeMinutes)
    {
        _signingKey = signingKey;
        _expirationTimeMinutes = expirationTimeMinutes;
    }

    public string Generate(User user)
    {
        throw new NotImplementedException();
    }
}
