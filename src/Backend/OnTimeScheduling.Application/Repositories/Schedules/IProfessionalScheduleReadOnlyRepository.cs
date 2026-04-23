using OnTimeScheduling.Domain.Entities.Schedules;

namespace OnTimeScheduling.Application.Repositories.Schedules;

public interface IProfessionalScheduleReadOnlyRepository
{
    Task<bool> HasOverlappingSchedule(
        Guid userId, 
        DayOfWeek dayOfWeek, 
        TimeSpan startTime, 
        TimeSpan endTime, 
        CancellationToken ct = default);

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
