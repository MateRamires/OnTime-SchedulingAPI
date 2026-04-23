namespace OnTimeScheduling.Application.UseCases.Clients;

public interface IDeleteClientUseCase
{
    Task ExecuteAsync(Guid clientId, CancellationToken ct = default);
}
