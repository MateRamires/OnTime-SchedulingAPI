namespace OnTimeScheduling.Communication.Responses.Appointments;

public class ResponseAgendaJson
{
    public DateTime RangeStartUtc { get; set; }
    public DateTime RangeEndUtc { get; set; }
    public List<ResponseAppointmentAgendaItemJson> Items { get; set; } = [];

}
