namespace OnTimeScheduling.Application.UseCases.Services;

public interface IActivateServiceUseCase
{
    Task ExecuteAsync(Guid serviceId, CancellationToken ct = default);
}
