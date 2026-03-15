namespace OnTimeScheduling.Communication.Requests;

public class RequestRegisterAppointmentJson
{
    public Guid ProfessionalId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid LocationId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string ClientPhone { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
}
