using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Repositories.Clients;
using OnTimeScheduling.Domain.Entities.Clients;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;

public class ClientRepository : IClientWriteOnlyRepository, IClientReadOnlyRepository
{
    private readonly AppDbContext _dbContext;

    public ClientRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Client client, CancellationToken ct = default)
    {
        await _dbContext.Clients.AddAsync(client, ct);
    }

    public void Update(Client client)
    {
        _dbContext.Clients.Update(client);
    }

    public Task<bool> ExistsActiveByPhone(string phone, CancellationToken ct = default)
    {
        return _dbContext.Clients.AnyAsync(c => c.Phone == phone && c.Status == RecordStatus.Active, ct);
    }

    public Task<Client?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _dbContext.Clients.FirstOrDefaultAsync(c => c.Id == id && c.Status == RecordStatus.Active, ct);
    }

    public Task<List<Client>> GetAllActiveAsync(CancellationToken ct = default)
    {
        return _dbContext.Clients
            .Where(c => c.Status == RecordStatus.Active)
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
    }

}
