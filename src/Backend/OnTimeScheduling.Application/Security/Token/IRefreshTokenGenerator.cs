namespace OnTimeScheduling.Application.Security.Token;

public interface IRefreshTokenGenerator
{
    string Generate();
    string Hash(string token);

}
