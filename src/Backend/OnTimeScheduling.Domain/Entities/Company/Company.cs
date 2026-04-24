using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;
using System.Text.RegularExpressions;

namespace OnTimeScheduling.Domain.Entities.Company;

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

        var normalizedDocument = Regex.Replace(document, @"\D", string.Empty);
        if (!IsValidCnpj(normalizedDocument))
            throw new DomainRuleException("Document (CNPJ) is invalid.");

        Document = document;
    }

    private static bool IsValidCnpj(string cnpj)
    {
        if (cnpj.Length != 14)
            return false;

        if (cnpj.Distinct().Count() == 1)
            return false;

        var baseNumber = cnpj[..12];
        var firstCheckDigit = CalculateCheckDigit(baseNumber, new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });
        var secondCheckDigit = CalculateCheckDigit($"{baseNumber}{firstCheckDigit}", new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });

        return cnpj.EndsWith($"{firstCheckDigit}{secondCheckDigit}");
    }

    private static int CalculateCheckDigit(string number, IReadOnlyList<int> weights)
    {
        var sum = 0;
        for (var i = 0; i < weights.Count; i++)
            sum += (number[i] - '0') * weights[i];

        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
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
