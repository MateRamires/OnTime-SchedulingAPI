using OnTimeScheduling.Application.Repositories.Schedules;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.Schedules;

namespace OnTimeScheduling.Application.UseCases.Schedules.Mapper;

public class ProfessionalScheduleResponseMapper
{
    public static ResponseProfessionalScheduleJson Map(ProfessionalScheduleDetails details)
    {
        return Map(details.Schedule, details.ProfessionalName, details.LocationName);
    }

    public static ResponseProfessionalScheduleJson Map(
        ProfessionalSchedule schedule,
        string? professionalName = null,
        string? locationName = null)
    {
        return new ResponseProfessionalScheduleJson
        {
            Id = schedule.Id,
            ProfessionalId = schedule.UserId,
            ProfessionalName = professionalName ?? string.Empty,
            LocationId = schedule.LocationId,
            LocationName = locationName ?? string.Empty,
            DayOfWeek = schedule.DayOfWeek,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime,
            CreatedAt = schedule.CreatedAt,
            UpdatedAt = schedule.UpdatedAt
        };
    }
}
