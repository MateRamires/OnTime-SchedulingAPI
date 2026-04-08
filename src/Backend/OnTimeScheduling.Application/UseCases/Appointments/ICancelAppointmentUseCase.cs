namespace OnTimeScheduling.Application.UseCases.Appointments;

public interface ICancelAppointmentUseCase
{
    Task ExecuteAsync(Guid appointmentId, CancellationToken ct = default);
}
