using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Domain.Entities.Clients;

public class Client : TenantEntity
{
    public string Name { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string? Email { get; private set; }
    public RecordStatus Status { get; private set; }

    private Client() { }

    public Client(string name, string phone, string? email)
    {
        SetName(name);
        SetPhone(phone);
        SetEmail(email);
        Status = RecordStatus.Active;
    }

    public void Update(string name, string phone, string? email)
    {
        SetName(name);
        SetPhone(phone);
        SetEmail(email);
        Touch();
    }

    public void Inactivate()
    {
        Status = RecordStatus.Inactive;
        Touch();
    }

    public void Activate()
    {
        Status = RecordStatus.Active;
        Touch();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainRuleException("Client name is required.");

        Name = name.Trim();
    }

    private void SetPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainRuleException("Client phone is required.");

        Phone = phone.Trim();
    }

    private void SetEmail(string? email)
    {
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
    }

}
