using OnTimeScheduling.Communication.Enums;

namespace OnTimeScheduling.Communication.Requests.Appointments;

public class RequestGetDailyAgendaJson
{
    public DateOnly Date { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? ProfessionalId { get; set; }
    public AppointmentStatus? Status { get; set; }

}
