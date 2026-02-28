namespace OnTimeScheduling.Application.Repositories.Schedules;

public interface IProfessionalScheduleReadOnlyRepository
{
    Task<bool> HasOverlappingSchedule(
        Guid userId, 
        DayOfWeek dayOfWeek, 
        TimeSpan startTime, 
        TimeSpan endTime, 
        CancellationToken ct = default);
}
