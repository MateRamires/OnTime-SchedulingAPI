using OnTimeScheduling.Domain.Entities.DefaultEntity;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Domain.Entities.ScheduleBlocks;

public class ScheduleBlock : TenantEntity
{
    public Guid? ProfessionalId { get; private set; }
    public Guid? LocationId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string Reason { get; private set; } = string.Empty;

    public User.User? Professional { get; private set; }
    public Locations.Location? Location { get; private set; }

    private ScheduleBlock() { }

    public ScheduleBlock(Guid? professionalId, Guid? locationId, DateTime startTime, DateTime endTime, string? reason)
    {
        SetScope(professionalId, locationId);
        SetTimes(startTime, endTime);
        SetReason(reason);
    }

    public void Update(Guid? professionalId, Guid? locationId, DateTime startTime, DateTime endTime, string? reason)
    {
        SetScope(professionalId, locationId);
        SetTimes(startTime, endTime);
        SetReason(reason);
        Touch();
    }

    private void SetScope(Guid? professionalId, Guid? locationId)
    {
        if (!professionalId.HasValue && !locationId.HasValue)
            throw new DomainRuleException("A schedule block must target a professional, a location, or both.");

        ProfessionalId = professionalId;
        LocationId = locationId;
    }

    private void SetTimes(DateTime startTime, DateTime endTime)
    {
        if (startTime.Kind != DateTimeKind.Utc || endTime.Kind != DateTimeKind.Utc)
            throw new DomainRuleException("Schedule block times must be in UTC.");

        if (startTime >= endTime)
            throw new DomainRuleException("The schedule block start time must be before the end time.");

        StartTime = startTime.ToUniversalTime();
        EndTime = endTime.ToUniversalTime();
    }

    private void SetReason(string? reason)
    {
        Reason = string.IsNullOrWhiteSpace(reason) ? string.Empty : reason.Trim();
    }

}
