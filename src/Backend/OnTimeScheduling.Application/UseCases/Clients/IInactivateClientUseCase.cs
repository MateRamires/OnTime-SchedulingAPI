namespace OnTimeScheduling.Application.UseCases.Clients;

public interface IInactivateClientUseCase
{
    Task ExecuteAsync(Guid clientId, CancellationToken ct = default);
}
