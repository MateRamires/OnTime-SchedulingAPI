using OnTimeScheduling.Application.Security.Models;

namespace OnTimeScheduling.Application.Security.Token;

public interface ILoggedUser
{
    LoggedUserInfo GetUser();
}
