namespace OnTimeScheduling.Application.UseCases.Locations;

public interface IActivateLocationUseCase
{
    Task ExecuteAsync(Guid locationId, CancellationToken ct = default);
}
