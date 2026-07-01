namespace OnTimeScheduling.Application.Security.Concurrency;

public interface IAgendaConcurrencyGuard
{
    Task ExecuteAsync(
        IEnumerable<AgendaConcurrencyLockKey> lockKeys,
        Func<CancellationToken, Task> operation,
        CancellationToken ct = default);

    Task<T> ExecuteAsync<T>(
        IEnumerable<AgendaConcurrencyLockKey> lockKeys,
        Func<CancellationToken, Task<T>> operation,
        CancellationToken ct = default);
}
