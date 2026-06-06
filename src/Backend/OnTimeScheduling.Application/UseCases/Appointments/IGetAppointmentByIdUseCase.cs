using OnTimeScheduling.Communication.Responses.Appointments;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public interface IGetAppointmentByIdUseCase
{
    Task<ResponseAppointmentJson> ExecuteAsync(Guid appointmentId, CancellationToken ct = default);
}
