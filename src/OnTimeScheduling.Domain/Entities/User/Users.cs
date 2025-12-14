using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Domain.Entities.User;

public class User : BaseEntity
{
    //Nullable because SUPER_ADMIN doesn't have a company.
    public Guid? CompanyId { get; private set; }

    public string Name { get; private set; } = null;
    public string Email { get; private set; } = null;
    public string Password { get; private set; } = null;

    public UserRole Role { get; private set; }
    public RecordStatus Status { get; private set; }

    private User() { }

    public User(Guid? companyId, string name, string email, string password, UserRole role) 
    { 
        CompanyId = companyId;
        SetName(name);
        SetEmail(email);
        SetPassword(password);
        Role = role;
        Status = RecordStatus.Active;

        if (Role != UserRole.SUPER_ADMIN && CompanyId is null)
            throw new InvalidOperationException("Non-Super_Admin users must have a companyId.");
        
    }

    private void SetPassword(string password)
    {
        password = (password ?? "").Trim();
        if (password.Length < 6) throw new ArgumentException("Invalid Password.");
        Password = password;
        Touch();
    }

    private void SetEmail(string email)
    {
        email = (email ?? "").Trim();
        if (email.Length < 5 || !email.Contains('@')) throw new ArgumentException("Invalid Email.");
        Email = email;
        Touch();
    }

    private void SetName(string name)
    {
        name = (name ?? "").Trim();
        if (name.Length < 2) throw new ArgumentException("Invalid Name.");
        Name = name;
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
}
