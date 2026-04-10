using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public interface IUpdateProviderAppointmentStatusUseCase
{
    Task ExecuteAsync(Guid appointmentId, RequestUpdateProviderAppointmentStatusJson request, CancellationToken ct = default);
}
