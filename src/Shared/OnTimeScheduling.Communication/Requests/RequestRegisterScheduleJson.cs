namespace OnTimeScheduling.Communication.Requests;

public class RequestRegisterScheduleJson
{
    public Guid UserId { get; set; }
    public Guid LocationId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    //JSON Expected Format: "08:00:00" or "08:00"
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
