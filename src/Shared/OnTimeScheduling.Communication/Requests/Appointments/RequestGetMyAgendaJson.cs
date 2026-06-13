using OnTimeScheduling.Communication.Enums;

namespace OnTimeScheduling.Communication.Requests.Appointments;

public class RequestGetMyAgendaJson
{
    public DateOnly Date { get; set; }
    public AgendaWindow Window { get; set; } = AgendaWindow.DAY;
    public Guid? LocationId { get; set; }
    public AppointmentStatus? Status { get; set; }

}
