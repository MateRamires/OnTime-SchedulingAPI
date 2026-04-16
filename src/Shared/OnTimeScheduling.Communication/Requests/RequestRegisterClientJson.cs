namespace OnTimeScheduling.Communication.Requests;

public class RequestRegisterClientJson
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }

}
