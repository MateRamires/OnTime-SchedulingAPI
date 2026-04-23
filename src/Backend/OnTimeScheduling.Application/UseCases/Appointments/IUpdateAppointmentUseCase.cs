using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public interface IUpdateAppointmentUseCase
{
    Task ExecuteAsync(Guid appointmentId, RequestUpdateAppointmentJson request, CancellationToken ct = default);
}
