using OnTimeScheduling.Communication.Enums;

namespace OnTimeScheduling.Communication.Requests;

public class RequestUpdateProviderAppointmentStatusJson
{
    public AppointmentOutcomeStatus Status { get; set; }
}
