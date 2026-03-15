using OnTimeScheduling.Domain.Entities.Services;

namespace OnTimeScheduling.Application.Repositories.Services;

public interface IServiceReadOnlyRepository
{
    Task<bool> ExistsActiveWithName(string name, CancellationToken ct = default);
    Task<bool> ExistsActiveById(Guid serviceId, CancellationToken ct = default);
    Task<Service?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
