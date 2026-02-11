namespace OnTimeScheduling.Domain.Entities.DefaultEntity;

public interface IMustHaveTenant
{
    Guid CompanyId { get; set; }
}
