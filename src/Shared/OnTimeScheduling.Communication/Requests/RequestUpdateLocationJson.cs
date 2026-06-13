namespace OnTimeScheduling.Communication.Requests;

public class RequestUpdateLocationJson
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? TimeZoneId { get; set; }

}
