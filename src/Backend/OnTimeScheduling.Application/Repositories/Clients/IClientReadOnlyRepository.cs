using OnTimeScheduling.Domain.Entities.Clients;

namespace OnTimeScheduling.Application.Repositories.Clients;

public interface IClientReadOnlyRepository
{
    Task<bool> ExistsActiveByPhone(string phone, CancellationToken ct = default);
    Task<bool> ExistsActiveByPhoneExceptId(string phone, Guid clientId, CancellationToken ct = default);
    Task<Client?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(List<Client> Items, int TotalItems)> GetAllActiveAsync(int skip, int take, CancellationToken ct = default);
}
