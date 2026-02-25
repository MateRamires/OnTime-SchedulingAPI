using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Domain.Entities.Schedules;

public class ProfessionalSchedule : TenantEntity
{
    public Guid UserId { get; private set; }
    public Guid LocationId { get; private set; }

    public DayOfWeek DayOfWeek { get; private set; }

    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }

    public User.User Professional { get; private set; } = null!;
    public Locations.Location Location { get; private set; } = null!;

    private ProfessionalSchedule() { }

    public ProfessionalSchedule(
        Guid userId,
        Guid locationId,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime)
    {
        UserId = userId;
        LocationId = locationId;
        DayOfWeek = dayOfWeek;

        SetTimes(startTime, endTime);
    }

    private void SetTimes(TimeSpan start, TimeSpan end)
    {
        if (start >= end)
            throw new DomainRuleException("The start time must be before the end time.");

        if (start.TotalHours < 0 || end.TotalHours > 24)
            throw new DomainRuleException("Invalid time format.");

        StartTime = start;
        EndTime = end;
    }
}
