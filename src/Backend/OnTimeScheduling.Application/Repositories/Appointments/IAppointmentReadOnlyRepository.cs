using OnTimeScheduling.Domain.Entities.Appointments;

namespace OnTimeScheduling.Application.Repositories.Appointments;

public interface IAppointmentReadOnlyRepository
{
    Task<bool> HasOverlappingAppointment(
        Guid professionalId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken ct = default);
    Task<List<Appointment>> GetAppointmentsByPeriod(
        Guid professionalId,
        DateTime startPeriod,
        DateTime endPeriod,
        CancellationToken ct = default);
}
