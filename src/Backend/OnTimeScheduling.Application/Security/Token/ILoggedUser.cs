using OnTimeScheduling.Domain.Entities.User;

namespace OnTimeScheduling.Application.Security.Token;

public interface ILoggedUser
{
    User User();
}
