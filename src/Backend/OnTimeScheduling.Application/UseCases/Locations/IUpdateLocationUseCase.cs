using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.UseCases.Locations;

public interface IUpdateLocationUseCase
{
    Task ExecuteAsync(Guid locationId, RequestUpdateLocationJson request, CancellationToken ct = default);
}
