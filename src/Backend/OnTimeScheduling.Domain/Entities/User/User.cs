using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Domain.Entities.User;

public class User : BaseEntity
{
    public Guid? CompanyId { get; private set; }

    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;

    public UserRole Role { get; private set; }
    public RecordStatus Status { get; private set; }

    private User() { }

    public User(Guid? companyId, string name, string email, string passwordHash, UserRole role) 
    {
        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new DomainRuleException("Invalid user role.");

        if (role == UserRole.SUPER_ADMIN && companyId is not null)
            throw new DomainRuleException("Super_Admin users must not have a companyId.");

        if (role != UserRole.SUPER_ADMIN && companyId is null)
            throw new DomainRuleException("Non-Super_Admin users must have a companyId.");

        CompanyId = companyId;
        SetName(name);
        SetEmail(email);
        SetPasswordHash(passwordHash);
        Role = role;
        Status = RecordStatus.Active;
    }

    private void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainRuleException("Password is required.");  

        PasswordHash = passwordHash;

    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainRuleException("Email is required.");

        Email = email.Trim();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainRuleException("Name is required.");

        Name = name.Trim();
    }

    public void UpdateInternalProfile(string name, string email, UserRole role)
    {
        if (role == UserRole.SUPER_ADMIN)
            throw new DomainRuleException("Super_Admin role cannot be assigned through company user management.");

        if (CompanyId is null)
            throw new DomainRuleException("Only company users can be updated through company user management.");

        SetName(name);
        SetEmail(email);
        Role = role;
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

    public void UpdatePasswordHash(string newHash)
    {
        SetPasswordHash(newHash);
    }
}
