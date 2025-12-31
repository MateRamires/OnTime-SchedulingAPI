using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Domain.Entities.User;

public class User : BaseEntity
{
    //Nullable because SUPER_ADMIN doesn't have a company.
    public Guid? CompanyId { get; private set; }

    public string Name { get; private set; } = null;
    public string Email { get; private set; } = null;
    public string PasswordHash { get; private set; } = null;

    public UserRole Role { get; private set; }
    public RecordStatus Status { get; private set; }

    private User() { }

    public User(Guid? companyId, string name, string email, string passwordHash, UserRole role) 
    {
        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new ArgumentException("Invalid user role.", nameof(role)); //TODO: Understand what this code means

        CompanyId = companyId;
        SetName(name);
        SetEmail(email);
        SetPasswordHash(passwordHash);
        Role = role;
        Status = RecordStatus.Active;

        if (Role != UserRole.SUPER_ADMIN && CompanyId is null)
            throw new InvalidOperationException("Non-Super_Admin users must have a companyId.");
        
    }

    private void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new InvalidOperationException("Password is required.");  //TODO: Check what invariants are actually necessary for Domain.Entity

        PasswordHash = passwordHash;

    }

    private void SetEmail(string email)
    {
        email = (email ?? "").Trim();
        if (email.Length < 5 || !email.Contains('@')) throw new ArgumentException("Invalid Email.");
        Email = email;

    }

    private void SetName(string name)
    {
        name = (name ?? "").Trim();
        if (name.Length < 2) throw new ArgumentException("Invalid Name.");
        Name = name;

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
