using Microsoft.AspNetCore.Http;
using OnTimeScheduling.Application.Security.Models;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;
using System.Security.Claims;

namespace OnTimeScheduling.Infrastructure.Security.Tokens;

public class LoggedUser : ILoggedUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public LoggedUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public LoggedUserInfo GetUser()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext?.User.Identity?.IsAuthenticated != true)
            throw new InvalidLoginException("The authenticated session is invalid.");

        var userIdClaim = httpContext.User.FindFirstValue(ClaimTypes.Sid);
        var userName = httpContext.User.FindFirstValue(ClaimTypes.Name);
        var userRoleClaim = httpContext.User.FindFirstValue(ClaimTypes.Role);
        var companyIdClaim = httpContext.User.FindFirstValue("CompanyId");

        if (!Guid.TryParse(userIdClaim, out var userId) ||
            string.IsNullOrWhiteSpace(userName) ||
            !Enum.TryParse<UserRole>(userRoleClaim, ignoreCase: true, out var userRole))
        {
            throw new InvalidLoginException("The authenticated session is invalid.");
        }

        Guid? companyId = null;
        if (!string.IsNullOrWhiteSpace(companyIdClaim))
        {
            if (!Guid.TryParse(companyIdClaim, out var parsedCompanyId))
                throw new InvalidLoginException("The authenticated session is invalid.");

            companyId = parsedCompanyId;
        }

        return new LoggedUserInfo
        {
            Id = userId,
            Name = userName,
            Role = userRole,
            CompanyId = companyId
        };
    }
}
