namespace OnTimeScheduling.Application.Security.Token;

public interface IRefreshTokenSettings
{
    uint ExpirationDays { get; }
}
