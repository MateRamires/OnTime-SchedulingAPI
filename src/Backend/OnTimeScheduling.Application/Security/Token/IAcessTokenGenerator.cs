using OnTimeScheduling.Domain.Entities.User;

namespace OnTimeScheduling.Application.Security.Token;

public interface IAcessTokenGenerator
{
    string Generate(User user);
}
