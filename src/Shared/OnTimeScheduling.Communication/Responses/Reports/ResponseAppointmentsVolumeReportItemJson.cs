using OnTimeScheduling.Communication.Enums;

namespace OnTimeScheduling.Communication.Responses.Reports;

public class ResponseAppointmentsVolumeReportItemJson
{
    public DateTime PeriodStartUtc { get; set; }
    public DateTime PeriodEndUtc { get; set; }
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public Guid ProfessionalId { get; set; }
    public string ProfessionalName { get; set; } = string.Empty;
    public Guid ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; }
    public int AppointmentsCount { get; set; }
    public int TotalDurationInMinutes { get; set; }
}
