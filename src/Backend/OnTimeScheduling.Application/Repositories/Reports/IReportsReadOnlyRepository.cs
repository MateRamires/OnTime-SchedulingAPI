using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.Repositories.Reports;

public interface IReportsReadOnlyRepository
{
    Task<List<AppointmentReportDetails>> GetAppointmentsStartedInPeriodAsync(
        DateTime startUtc,
        DateTime endUtc,
        Guid? locationId,
        Guid? professionalId,
        Guid? serviceId,
        IReadOnlyCollection<AppointmentStatus>? statuses,
        CancellationToken ct = default);

    Task<List<AppointmentReportDetails>> GetAppointmentsOverlappingPeriodAsync(
        DateTime startUtc,
        DateTime endUtc,
        Guid? locationId,
        Guid? professionalId,
        CancellationToken ct = default);

    Task<List<ProfessionalOccupancyScheduleDetails>> GetProfessionalSchedulesForOccupancyAsync(
        Guid? locationId,
        Guid? professionalId,
        CancellationToken ct = default);

    Task<List<ProfessionalOccupancyScheduleBlockDetails>> GetScheduleBlocksForOccupancyAsync(
        DateTime startUtc,
        DateTime endUtc,
        Guid? locationId,
        Guid? professionalId,
        CancellationToken ct = default);
}
