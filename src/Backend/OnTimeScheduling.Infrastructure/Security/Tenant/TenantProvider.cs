using Microsoft.AspNetCore.Http;
using OnTimeScheduling.Application.Security.Tenant;

namespace OnTimeScheduling.Infrastructure.Security.Tenant;

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? CompanyId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;

            if (context == null) return null;

            var claimValue = context.User.FindFirst("CompanyId")?.Value;

            if (Guid.TryParse(claimValue, out var companyId))
            {
                return companyId;
            }

            return null;
        }
    }
}
