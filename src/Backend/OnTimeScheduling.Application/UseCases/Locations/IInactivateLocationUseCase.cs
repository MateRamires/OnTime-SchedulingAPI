namespace OnTimeScheduling.Application.UseCases.Locations;

public interface IInactivateLocationUseCase
{
    Task ExecuteAsync(Guid locationId, CancellationToken ct = default);
}
