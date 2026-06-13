namespace OnTimeScheduling.Communication.Requests;

public class RequestGetProfessionalSchedulesJson : RequestPaginationQuery
{
    public Guid? ProfessionalId { get; set; }
    public Guid? LocationId { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
}
