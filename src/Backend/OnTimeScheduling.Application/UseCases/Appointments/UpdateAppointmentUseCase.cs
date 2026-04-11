
using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public class UpdateAppointmentUseCase : IUpdateAppointmentUseCase
{
    public Task ExecuteAsync(Guid appointmentId, RequestUpdateAppointmentJson request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
