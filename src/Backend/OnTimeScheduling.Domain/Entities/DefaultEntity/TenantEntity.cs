
namespace OnTimeScheduling.Domain.Entities.DefaultEntity;

public class TenantEntity : BaseEntity, IMustHaveTenant
{
    public Guid CompanyId { get; set; }
}
