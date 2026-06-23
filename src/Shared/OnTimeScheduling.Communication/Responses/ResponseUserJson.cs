namespace OnTimeScheduling.Communication.Responses;

public class ResponseUserJson
{
    public Guid Id { get; set; }
    public Guid? CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Enums.UserRole Role { get; set; }
    public Enums.RecordStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}
