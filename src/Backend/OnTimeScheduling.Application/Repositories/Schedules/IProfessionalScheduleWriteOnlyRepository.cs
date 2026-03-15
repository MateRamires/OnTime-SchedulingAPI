using OnTimeScheduling.Domain.Entities.Schedules;

namespace OnTimeScheduling.Application.Repositories.Schedules;

public interface IProfessionalScheduleWriteOnlyRepository
{
    Task Add(ProfessionalSchedule schedule, CancellationToken ct = default);
}
