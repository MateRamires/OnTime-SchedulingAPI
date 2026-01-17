using Microsoft.IdentityModel.Tokens;
using OnTimeScheduling.Application.Security; // Verifique se o namespace da interface está correto
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Domain.Entities.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Sid, user.Id.ToString()), 
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        if (user.CompanyId.HasValue)
        {
            claims.Add(new Claim("TenantId", user.CompanyId.Value.ToString()));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_expirationTimeMinutes),
            SigningCredentials = new SigningCredentials(
                SigningKey(),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(securityToken);
    }

    private SymmetricSecurityKey SigningKey()
    {
        var key = Encoding.UTF8.GetBytes(_signingKey);
        return new SymmetricSecurityKey(key);
    }
}