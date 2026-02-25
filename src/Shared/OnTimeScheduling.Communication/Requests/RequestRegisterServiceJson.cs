namespace OnTimeScheduling.Communication.Requests;

public class RequestRegisterServiceJson
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationInMinutes { get; set; }
}
