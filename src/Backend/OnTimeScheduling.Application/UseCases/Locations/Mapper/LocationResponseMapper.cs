using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.Locations;

namespace OnTimeScheduling.Application.UseCases.Locations.Mapper;

public class LocationResponseMapper
{
    public static ResponseLocationJson Map(Location location)
    {
        return new ResponseLocationJson
        {
            Id = location.Id,
            Name = location.Name,
            Address = location.Address,
            TimeZoneId = location.TimeZoneId,
            Status = location.Status.ToString(),
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt
        };
    }

}
