using OnTimeScheduling.Domain.Entities.Schedules;

namespace OnTimeScheduling.Application.Repositories.Schedules;

public interface IProfessionalScheduleReadOnlyRepository
{
    Task<ProfessionalSchedule?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<ProfessionalScheduleDetails?> GetDetailsByIdAsync(Guid id, CancellationToken ct = default);

    Task<(List<ProfessionalScheduleDetails> Items, int TotalItems)> GetAllAsync(
        int skip,
        int take,
        Guid? professionalId,
        Guid? locationId,
        DayOfWeek? dayOfWeek,
        CancellationToken ct = default);

    Task<bool> HasOverlappingSchedule(
        Guid userId, 
        DayOfWeek dayOfWeek, 
        TimeSpan startTime, 
        TimeSpan endTime, 
        CancellationToken ct = default,
        Guid? ignoredScheduleId = null);

    Task<bool> HasCoverageForAppointment(
        Guid userId,
        Guid locationId,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        CancellationToken ct = default);

    Task<List<ProfessionalSchedule>> GetSchedulesByDayAsync(
        Guid userId,
        Guid locationId,
        DayOfWeek dayOfWeek,
        CancellationToken ct = default);

}
