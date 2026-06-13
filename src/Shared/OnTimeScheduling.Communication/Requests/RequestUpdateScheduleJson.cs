namespace OnTimeScheduling.Communication.Requests;

public class RequestUpdateScheduleJson
{
    public Guid UserId { get; set; }
    public Guid LocationId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
