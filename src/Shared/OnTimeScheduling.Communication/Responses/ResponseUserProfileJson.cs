using OnTimeScheduling.Communication.Enums;

namespace OnTimeScheduling.Communication.Responses;

public class ResponseUserProfileJson
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? CompanyId { get; set; }
    public string Role { get; set; } = string.Empty;
}
