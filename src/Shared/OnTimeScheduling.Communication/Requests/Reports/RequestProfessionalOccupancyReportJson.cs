namespace OnTimeScheduling.Communication.Requests.Reports;

public class RequestProfessionalOccupancyReportJson
{
    public DateTime? StartTimeUtc { get; set; }
    public DateTime? EndTimeUtc { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? ProfessionalId { get; set; }
}
