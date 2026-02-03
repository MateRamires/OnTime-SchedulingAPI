using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Domain.Entities.User;

public class Company : BaseEntity
{
    public string SocialReason { get; private set; } = null!; 
    public string FantasyName { get; private set; } = null!;  
    public string Document { get; private set; } = null!;     
    public string Phone { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public RecordStatus Status { get; private set; }

    private Company() { }

    public Company(string socialReason, string fantasyName, string document, string phone, string email)
    {
        SetSocialReason(socialReason);
        SetFantasyName(fantasyName);
        SetDocument(document);
        SetPhone(phone);
        SetEmail(email);

        Status = RecordStatus.Active;
    }

    private void SetSocialReason(string socialReason)
    {
        if (string.IsNullOrWhiteSpace(socialReason))
            throw new DomainRuleException("Social Reason is required.");
        SocialReason = socialReason;
    }

    private void SetFantasyName(string fantasyName)
    {
        if (string.IsNullOrWhiteSpace(fantasyName))
            throw new DomainRuleException("Fantasy Name is required.");
        FantasyName = fantasyName;
    }

    private void SetDocument(string document)
    {
        if (string.IsNullOrWhiteSpace(document))
            throw new DomainRuleException("Document (CNPJ) is required.");
        //TODO: Adicionar validação de regex/algoritmo de CNPJ.
        Document = document;
    }

    private void SetPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainRuleException("Phone is required.");
        Phone = phone;
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainRuleException("Email is required.");
        Email = email;
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
