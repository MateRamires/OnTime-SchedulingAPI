using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.Security.Models;

public class LoggedUserInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? CompanyId { get; set; }
    public UserRole Role { get; set; }
}
