namespace OnTimeScheduling.Communication.Responses.Appointments;

public class ResponseAppointmentClientJson : ResponseAppointmentParticipantJson
{
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }

}
