namespace OnTimeScheduling.Application.Security.Token;

public interface IAcessTokenGenerator
{
    string Generate(string userIdentifier);
}
