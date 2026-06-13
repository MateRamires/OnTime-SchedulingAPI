namespace OnTimeScheduling.Application.UseCases.Services;

public interface IInactivateServiceUseCase
{
    Task ExecuteAsync(Guid serviceId, CancellationToken ct = default);
}
