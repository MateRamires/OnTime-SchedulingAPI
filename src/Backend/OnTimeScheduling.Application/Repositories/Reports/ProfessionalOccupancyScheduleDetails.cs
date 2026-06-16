namespace OnTimeScheduling.Application.Repositories.Reports;

public class ProfessionalOccupancyScheduleDetails
{
    public Guid ProfessionalId { get; set; }
    public string ProfessionalName { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string LocationTimeZoneId { get; set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
