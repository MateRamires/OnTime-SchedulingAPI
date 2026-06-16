namespace OnTimeScheduling.Application.Repositories.Reports;

public class ProfessionalOccupancyScheduleBlockDetails
{
    public Guid? ProfessionalId { get; set; }
    public Guid? LocationId { get; set; }
    public DateTime StartTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
}
