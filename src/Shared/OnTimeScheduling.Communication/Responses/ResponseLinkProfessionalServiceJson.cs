namespace OnTimeScheduling.Communication.Responses;

public class ResponseLinkProfessionalServiceJson
{
    public Guid UserId { get; set; }
    public Guid ServiceId { get; set; }
    public string Message { get; set; } = string.Empty;
}
