namespace OnTimeScheduling.Communication.Requests;

public class RequestGetScheduleBlocksJson : RequestPaginationQuery
{
    public Guid? ProfessionalId { get; set; }
    public Guid? LocationId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IncludeExpired { get; set; }

}
