namespace OnTimeScheduling.Communication.Responses;

public class ResponseScheduleBlockJson
{
    public Guid Id { get; set; }
    public Guid? ProfessionalId { get; set; }
    public string? ProfessionalName { get; set; }
    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}
