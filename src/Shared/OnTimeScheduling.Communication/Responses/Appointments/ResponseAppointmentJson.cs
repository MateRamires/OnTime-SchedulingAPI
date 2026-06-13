using OnTimeScheduling.Communication.Enums;

namespace OnTimeScheduling.Communication.Responses.Appointments;

public class ResponseAppointmentJson
{
    public Guid Id { get; set; }
    public ResponseAppointmentClientJson Client { get; set; } = new();
    public ResponseAppointmentParticipantJson Professional { get; set; } = new();
    public ResponseAppointmentParticipantJson Location { get; set; } = new();
    public ResponseAppointmentServiceJson Service { get; set; } = new();
    public AppointmentStatus Status { get; set; }
    public DateTime StartTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

}
