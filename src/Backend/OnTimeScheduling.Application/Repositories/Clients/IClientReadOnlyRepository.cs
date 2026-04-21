using OnTimeScheduling.Domain.Entities.Clients;

namespace OnTimeScheduling.Application.Repositories.Clients;

public interface IClientReadOnlyRepository
{
    Task<bool> ExistsActiveByPhone(string phone, CancellationToken ct = default);
    Task<Client?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Client>> GetAllActiveAsync(CancellationToken ct = default);

}
