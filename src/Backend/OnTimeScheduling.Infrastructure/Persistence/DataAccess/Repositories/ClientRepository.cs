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

    public Task<bool> ExistsActiveByPhoneExceptId(string phone, Guid clientId, CancellationToken ct = default)
    {
        return _dbContext.Clients.AnyAsync(c =>
            c.Phone == phone &&
            c.Status == RecordStatus.Active &&
            c.Id != clientId, ct);
    }


    public Task<Client?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _dbContext.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.Status == RecordStatus.Active, ct);

    }

    public async Task<(List<Client> Items, int TotalItems)> GetAllActiveAsync(int skip, int take, CancellationToken ct = default)
    {
        var query = _dbContext.Clients
            .AsNoTracking()
            .Where(c => c.Status == RecordStatus.Active);

        var totalItems = await query.CountAsync(ct);

        var items = await query
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

        return (items, totalItems);

    }

}
