namespace OnTimeScheduling.Application.Repositories.Services;

public interface IServiceReadOnlyRepository
{
    Task<bool> ExistsActiveWithName(string name, CancellationToken ct = default);
}
