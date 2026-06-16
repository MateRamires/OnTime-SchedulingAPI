namespace OnTimeScheduling.Communication.Responses.Reports;

public class ResponseProfessionalOccupancyReportItemJson
{
    public Guid ProfessionalId { get; set; }
    public string ProfessionalName { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public int ScheduledCapacityInMinutes { get; set; }
    public int BlockedCapacityInMinutes { get; set; }
    public int AvailableCapacityInMinutes { get; set; }
    public int OccupiedInMinutes { get; set; }
    public int AppointmentsCount { get; set; }
    public decimal OccupancyPercentage { get; set; }
}
