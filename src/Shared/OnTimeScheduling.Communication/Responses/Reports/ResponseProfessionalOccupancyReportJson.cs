namespace OnTimeScheduling.Communication.Responses.Reports;

public class ResponseProfessionalOccupancyReportJson
{
    public DateTime StartTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
    public List<ResponseProfessionalOccupancyReportItemJson> Items { get; set; } = [];
}
