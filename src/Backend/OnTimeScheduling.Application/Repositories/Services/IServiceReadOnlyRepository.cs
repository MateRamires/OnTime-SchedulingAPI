namespace OnTimeScheduling.Application.Repositories.Services;

public interface IServiceReadOnlyRepository
{
    Task<bool> ExistsActiveWithName(string name, CancellationToken ct = default);
    Task<bool> ExistsActiveById(Guid serviceId, CancellationToken ct = default);
}
