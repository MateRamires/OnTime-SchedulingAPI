using OnTimeScheduling.Domain.Entities.Clients;

namespace OnTimeScheduling.Application.Repositories.Clients;

public interface IClientWriteOnlyRepository
{
    Task Add(Client client, CancellationToken ct = default);
    void Update(Client client);

}
