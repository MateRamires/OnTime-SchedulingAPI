using OnTimeScheduling.Application.Security.Token;

namespace OnTimeScheduling.Infrastructure.Security.Tokens;

public class RefreshTokenSettings : IRefreshTokenSettings
{
    public uint ExpirationDays { get; }

    public RefreshTokenSettings(uint expirationDays)
    {
        ExpirationDays = expirationDays;
    }

}
