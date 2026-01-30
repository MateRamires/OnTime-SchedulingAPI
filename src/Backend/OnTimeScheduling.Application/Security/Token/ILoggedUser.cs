using OnTimeScheduling.Application.Security.Models;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.Security.Token;

public interface ILoggedUser
{
    LoggedUserInfo GetUser();
}
