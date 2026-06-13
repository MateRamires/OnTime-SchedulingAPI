using OnTimeScheduling.Domain.Entities.Schedules;

namespace OnTimeScheduling.Application.Repositories.Schedules;

public class ProfessionalScheduleDetails
{
    public ProfessionalSchedule Schedule { get; set; } = null!;
    public string ProfessionalName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
}
