using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Domain.Entities.User;

public class User : Entity
{
    //Nullable because SUPER_ADMIN doesn't have a company.
    public Guid? CompanyId { get; private set; }

    public string Name { get; private set; } = null;
    public string Email { get; private set; } = null;
    public string Password { get; private set; } = null;

    public UserRole Role { get; private set; }
    public RecordStatus Status { get; private set; }

    private User() { }
}
