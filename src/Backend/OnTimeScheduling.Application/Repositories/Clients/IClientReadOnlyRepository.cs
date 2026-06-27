using OnTimeScheduling.Domain.Entities.Clients;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.Repositories.Clients;

public interface IClientReadOnlyRepository
{
    Task<bool> ExistsActiveByPhone(string phone, CancellationToken ct = default);
    Task<bool> ExistsActiveByPhoneExceptId(string phone, Guid clientId, CancellationToken ct = default);
    Task<Client?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Client?> GetActiveByIdAsync(Guid id, CancellationToken ct = default);
    Task<(List<Client> Items, int TotalItems)> GetAllAsync(
        int skip,
        int take,
        RecordStatus? status = null,
        string? searchTerm = null,
        CancellationToken ct = default);
}
