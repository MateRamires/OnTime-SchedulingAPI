namespace OnTimeScheduling.Communication.Requests;

public class RequestRegisterAppointmentJson
{
    public Guid ClientId { get; set; }
    public Guid ProfessionalId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid LocationId { get; set; }
    public DateTime StartTime { get; set; }
}
