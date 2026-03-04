using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Domain.Entities.Locations;

public class Location : TenantEntity
{
    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public RecordStatus Status { get; private set; }
    private Location() { }

    public Location(Guid companyId, string name, string address)
    {
        if (companyId == Guid.Empty)
            throw new DomainRuleException("A valid CompanyId is required for a Location.");

        CompanyId = companyId;
        SetName(name);
        SetAddress(address);
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

    public void Inactivate()
    {
        Status = RecordStatus.Inactive;
    }

    public void Activate()
    {
        Status = RecordStatus.Active;
    }
}
