using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.Services;

namespace OnTimeScheduling.Application.UseCases.Services.Mapper;

public class ServiceResponseMapper
{
    public static ResponseServiceJson Map(Service service) => new()
    {
        Id = service.Id,
        Name = service.Name,
        Description = service.Description,
        Price = service.Price,
        DurationInMinutes = service.DurationInMinutes,
        Status = service.Status.ToString(),
        CreatedAt = service.CreatedAt,
        UpdatedAt = service.UpdatedAt
    };

}
