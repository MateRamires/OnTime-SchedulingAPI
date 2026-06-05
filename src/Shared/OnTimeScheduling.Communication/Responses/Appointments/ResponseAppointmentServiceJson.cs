namespace OnTimeScheduling.Communication.Responses.Appointments;

public class ResponseAppointmentServiceJson : ResponseAppointmentParticipantJson
{
    public int DurationInMinutes { get; set; }
    public decimal Price { get; set; }

}
