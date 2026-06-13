using OnTimeScheduling.Domain.Entities.Services;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.Repositories.Services;

public interface IServiceReadOnlyRepository
{
    Task<bool> ExistsActiveWithName(string name, CancellationToken ct = default);
    Task<bool> ExistsActiveById(Guid serviceId, CancellationToken ct = default);
    Task<Service?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(List<Service> Items, int TotalItems)> GetAllAsync(int skip, int take, RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default);
    Task<bool> ExistsWithNameExceptId(string name, Guid serviceId, CancellationToken ct = default);

}
