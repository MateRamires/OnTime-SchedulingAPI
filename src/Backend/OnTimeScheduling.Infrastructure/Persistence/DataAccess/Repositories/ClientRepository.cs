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
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public Task<Client?> GetActiveByIdAsync(Guid id, CancellationToken ct = default)
    {
        return _dbContext.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.Status == RecordStatus.Active, ct);
    }

    public async Task<(List<Client> Items, int TotalItems)> GetAllAsync(
        int skip,
        int take,
        RecordStatus? status = null,
        string? searchTerm = null,
        CancellationToken ct = default)
    {
        var query = _dbContext.Clients
            .AsNoTracking()
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(normalizedSearchTerm) ||
                c.Phone.ToLower().Contains(normalizedSearchTerm) ||
                (c.Email != null && c.Email.ToLower().Contains(normalizedSearchTerm)));
        }

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
