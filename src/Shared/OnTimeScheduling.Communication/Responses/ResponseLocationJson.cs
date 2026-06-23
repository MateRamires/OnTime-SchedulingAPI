namespace OnTimeScheduling.Communication.Responses;

public class ResponseLocationJson
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string TimeZoneId { get; set; } = string.Empty;
    public Enums.RecordStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}
