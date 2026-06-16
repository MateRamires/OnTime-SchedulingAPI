using OnTimeScheduling.Communication.Enums;

namespace OnTimeScheduling.Communication.Requests.Reports;

public class RequestAppointmentsVolumeReportJson
{
    public DateTime? StartTimeUtc { get; set; }
    public DateTime? EndTimeUtc { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? ProfessionalId { get; set; }
    public Guid? ServiceId { get; set; }
    public List<AppointmentStatus>? Status { get; set; }
    public ReportPeriodGrouping GroupBy { get; set; } = ReportPeriodGrouping.DAY;
}
