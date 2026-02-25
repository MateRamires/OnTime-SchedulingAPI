namespace OnTimeScheduling.Application.Security.Tenant;

public interface ITenantProvider
{
    Guid? CompanyId { get; }
}
