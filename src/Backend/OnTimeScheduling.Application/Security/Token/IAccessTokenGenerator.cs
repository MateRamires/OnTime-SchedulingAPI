using OnTimeScheduling.Domain.Entities.User;

namespace OnTimeScheduling.Application.Security.Token;

public interface IAccessTokenGenerator
{
    string Generate(User user);
}
