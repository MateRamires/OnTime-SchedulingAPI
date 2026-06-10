namespace OnTimeScheduling.Communication.Requests;

public class RequestUpdateScheduleBlockJson
{
    public Guid? ProfessionalId { get; set; }
    public Guid? LocationId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Reason { get; set; }

}
