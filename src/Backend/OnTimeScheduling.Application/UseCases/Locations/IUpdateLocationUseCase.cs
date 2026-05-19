namespace OnTimeScheduling.Application.UseCases.Locations;

public class IUpdateLocationUseCase
{
    Task ExecuteAsync(Guid locationId, RequestUpdateLocationJson request, CancellationToken ct = default);
}
