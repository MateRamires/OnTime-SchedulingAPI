namespace OnTimeScheduling.Application.UseCases.Clients;

public interface IActivateClientUseCase
{
    Task ExecuteAsync(Guid clientId, CancellationToken ct = default);
}
