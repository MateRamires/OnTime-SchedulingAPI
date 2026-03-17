namespace OnTimeScheduling.Communication.Responses;

public class ResponseAvailableTimeSlotsJson
{
    //[ "2026-03-20T13:00:00Z", "2026-03-20T14:00:00Z" ]
    public List<DateTime> AvailableSlotsUtc { get; set; } = [];
}
