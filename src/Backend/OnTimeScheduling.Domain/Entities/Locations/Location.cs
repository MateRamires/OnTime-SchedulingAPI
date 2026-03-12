using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Domain.Entities.Locations;

public class Location : TenantEntity
{
    public const string DefaultTimeZoneId = "America/Sao_Paulo";
    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string TimeZoneId { get; private set; } = DefaultTimeZoneId;
    public RecordStatus Status { get; private set; }
    private Location() { }

    public Location(Guid companyId, string name, string address, string? timeZoneId = null)
    {
        if (companyId == Guid.Empty)
            throw new DomainRuleException("A valid CompanyId is required for a Location.");

        CompanyId = companyId;
        SetName(name);
        SetAddress(address);
        SetTimeZoneId(timeZoneId);
        Status = RecordStatus.Active;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainRuleException("Location name is required.");

        Name = name;
    }

    public void SetAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainRuleException("Address is required.");

        Address = address;
    }

    public void SetTimeZoneId(string? timeZoneId)
    {
        var normalizedTimeZoneId = string.IsNullOrWhiteSpace(timeZoneId)
            ? DefaultTimeZoneId
            : timeZoneId.Trim();

        if (normalizedTimeZoneId.Length > 100)
            throw new DomainRuleException("Time zone ID must have less than 100 characters.");

        TimeZoneId = normalizedTimeZoneId;
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
