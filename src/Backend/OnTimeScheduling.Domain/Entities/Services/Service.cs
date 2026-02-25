using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Domain.Entities.Services;

public class Service : TenantEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int DurationInMinutes { get; private set; }
    public RecordStatus Status { get; private set; }

    private Service() { }

    public Service(string name, string? description, decimal price, int durationInMinutes)
    {
        SetName(name);
        SetDescription(description);
        SetPrice(price);
        SetDuration(durationInMinutes);

        Status = RecordStatus.Active;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainRuleException("Service Name is required.");

        Name = name;
    }

    private void SetDescription(string? description)
    {
        Description = description;
    }

    private void SetPrice(decimal price)
    {
        if (price < 0)
            throw new DomainRuleException("Price cannot be negative.");

        Price = price;
    }

    private void SetDuration(int durationInMinutes)
    {
        if (durationInMinutes <= 0)
            throw new DomainRuleException("Duration must be greater than zero.");

        DurationInMinutes = durationInMinutes;
    }

    public void Inactivate()
    {
        Status = RecordStatus.Inactive;
    }

    public void Activate()
    {
        Status = RecordStatus.Active;
    }
}
