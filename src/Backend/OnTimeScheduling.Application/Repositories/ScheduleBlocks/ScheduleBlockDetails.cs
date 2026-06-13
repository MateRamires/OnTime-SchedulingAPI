using OnTimeScheduling.Domain.Entities.ScheduleBlocks;

namespace OnTimeScheduling.Application.Repositories.ScheduleBlocks;

public class ScheduleBlockDetails
{
    public ScheduleBlock Block { get; set; } = null!;
    public string? ProfessionalName { get; set; }
    public string? LocationName { get; set; }

}
