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

        if (httpContext == null || !httpContext.User.Identity.IsAuthenticated) 
        {
            throw new ErrorOnUnauthorizedException("User not authenticated");
        }

        var claims = httpContext.User.Claims;

        var userIdClaim = claims.First(c => c.Type == ClaimTypes.Sid).Value;
        var userId = Guid.Parse(userIdClaim);

        var userName = claims.First(c => c.Type == ClaimTypes.Name).Value;

        var userRoleClaim = claims.First(c => c.Type == ClaimTypes.Role).Value;
        var userRole = Enum.Parse<UserRole>(userRoleClaim);

        var companyIdClaim = claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
        Guid? companyId = companyIdClaim != null ? Guid.Parse(companyIdClaim) : null;

        return new LoggedUserInfo
        {
            Id = userId,
            Name = userName,
            Role = userRole,
            CompanyId = companyId
        };
    }
}
