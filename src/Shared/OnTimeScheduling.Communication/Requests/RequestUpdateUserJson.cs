using OnTimeScheduling.Communication.Enums;

namespace OnTimeScheduling.Communication.Requests;

public class RequestUpdateUserJson
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }

}
