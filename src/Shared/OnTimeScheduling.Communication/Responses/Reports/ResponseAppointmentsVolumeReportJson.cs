namespace OnTimeScheduling.Communication.Responses.Reports;

public class ResponseAppointmentsVolumeReportJson
{
    public DateTime StartTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
    public int TotalAppointments { get; set; }
    public List<ResponseAppointmentsVolumeReportItemJson> Items { get; set; } = [];
}
