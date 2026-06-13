using OnTimeScheduling.Application.Repositories.ScheduleBlocks;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.ScheduleBlocks;

namespace OnTimeScheduling.Application.UseCases.ScheduleBlocks.Mapper;

public class ScheduleBlockResponseMapper
{
    public static ResponseScheduleBlockJson Map(ScheduleBlockDetails details)
    {
        return Map(details.Block, details.ProfessionalName, details.LocationName);
    }

    public static ResponseScheduleBlockJson Map(ScheduleBlock block, string? professionalName = null, string? locationName = null)
    {
        return new ResponseScheduleBlockJson
        {
            Id = block.Id,
            ProfessionalId = block.ProfessionalId,
            ProfessionalName = professionalName,
            LocationId = block.LocationId,
            LocationName = locationName,
            StartTime = block.StartTime,
            EndTime = block.EndTime,
            Reason = block.Reason,
            CreatedAt = block.CreatedAt,
            UpdatedAt = block.UpdatedAt
        };
    }

}
