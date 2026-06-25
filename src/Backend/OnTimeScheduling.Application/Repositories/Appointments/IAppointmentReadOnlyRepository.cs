using OnTimeScheduling.Domain.Entities.Appointments;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.Repositories.Appointments;

public interface IAppointmentReadOnlyRepository
{
    Task<bool> HasOverlappingAppointment(
        Guid professionalId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken ct = default,
        Guid? ignoredAppointmentId = null);

    Task<List<Appointment>> GetAppointmentsByPeriod(
        Guid professionalId,
        DateTime startPeriod,
        DateTime endPeriod,
        CancellationToken ct = default);

    Task<List<Appointment>> GetAppointmentsByDateRangeAsync(
        Guid professionalId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken ct = default);

    Task<bool> HasOverlappingAppointmentForBlockAsync(
        Guid? professionalId,
        Guid? locationId,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        CancellationToken ct = default);

    Task<bool> HasFutureScheduledAppointmentsAsync(
        Guid? professionalId = null,
        Guid? locationId = null,
        Guid? serviceId = null,
        Guid? clientId = null,
        CancellationToken ct = default);

    Task<List<Appointment>> GetFutureScheduledAppointmentsForProfessionalLocationAsync(
        Guid professionalId,
        Guid locationId,
        CancellationToken ct = default);

    Task<Appointment?> GetAppointmentByIdAsync(Guid id, CancellationToken ct = default);

    Task<List<AppointmentAgendaItem>> GetAgendaAsync(
        DateTime startUtc,
        DateTime endUtc,
        Guid? locationId,
        Guid? professionalId,
        AppointmentStatus? status,
        CancellationToken ct = default);

    Task<AppointmentDetails?> GetAppointmentDetailsByIdAsync(Guid id, CancellationToken ct = default);

    Task<(List<AppointmentDetails> Items, int TotalItems)> GetAppointmentsAsync(
        int skip,
        int take,
        Guid? locationId,
        Guid? professionalId,
        Guid? clientId,
        Guid? serviceId,
        IReadOnlyCollection<AppointmentStatus>? statuses,
        DateTime? startTimeUtc,
        DateTime? endTimeUtc,
        CancellationToken ct = default);


}
