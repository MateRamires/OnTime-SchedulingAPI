using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Locations;

public interface IGetLocationByIdUseCase
{
    Task<ResponseLocationJson> ExecuteAsync(Guid locationId, CancellationToken ct = default);
}
