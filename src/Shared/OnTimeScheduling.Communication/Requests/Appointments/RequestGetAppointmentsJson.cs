using OnTimeScheduling.Communication.Enums;

namespace OnTimeScheduling.Communication.Requests.Appointments;

public class RequestGetAppointmentsJson : RequestPaginationQuery
{
    public Guid? LocationId { get; set; }
    public Guid? ProfessionalId { get; set; }
    public Guid? ClientId { get; set; }
    public Guid? ServiceId { get; set; }
    public List<AppointmentStatus>? Status { get; set; }
    public DateTime? StartTimeUtc { get; set; }
    public DateTime? EndTimeUtc { get; set; }

}
