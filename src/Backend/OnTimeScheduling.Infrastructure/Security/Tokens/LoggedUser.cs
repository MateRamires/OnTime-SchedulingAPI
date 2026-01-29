using Microsoft.AspNetCore.Http;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Domain.Entities.User;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Infrastructure.Security.Tokens;

public class LoggedUser : ILoggedUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public LoggedUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public User User()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null || !httpContext.User.Identity.IsAuthenticated) 
        {
            throw new ErrorOnUnauthorizedException("User not authenticated");
        }

        var claims = httpContext.User.Claims;
    }
}
