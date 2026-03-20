namespace OnTimeScheduling.Communication.Requests;

public class RequestGetAvailableTimeSlotsJson
{
    public Guid ProfessionalId { get; set; }
    public Guid LocationId { get; set; }
    public Guid ServiceId { get; set; }

    public DateOnly TargetDate { get; set; }
}
