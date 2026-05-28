using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Domain.Entities.Services;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;

public class ServiceRepository : IServiceWriteOnlyRepository, IServiceReadOnlyRepository
{
    private readonly AppDbContext _dbContext;

    public ServiceRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    //TODO: checar necessidade de colocar AsNoTracking em algum dos metodos

    public async Task Add(Service service, CancellationToken ct = default)
    {
        await _dbContext.Services.AddAsync(service, ct);
    }

    public async Task<bool> ExistsActiveWithName(string name, CancellationToken ct = default)
    {
        return await _dbContext.Services
            .AnyAsync(s => s.Name.ToLower() == name.ToLower() && s.Status == RecordStatus.Active, ct);
    }

    public void Update(Service service)
    {
        _dbContext.Services.Update(service);
    }


    public async Task<bool> ExistsActiveById(Guid serviceId, CancellationToken ct = default)
    {
        return await _dbContext.Services
            .AnyAsync(s => s.Id == serviceId && s.Status == RecordStatus.Active, ct);
    }

    public async Task<Service?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Services
             .FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<(List<Service> Items, int TotalItems)> GetAllAsync(int skip, int take, RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default)
    {
        var query = _dbContext.Services.AsNoTracking().AsQueryable();

        if (status.HasValue)
            query = query.Where(service => service.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalized = searchTerm.Trim().ToLower();
            query = query.Where(service =>
                service.Name.ToLower().Contains(normalized) ||
                (service.Description != null && service.Description.ToLower().Contains(normalized)));
        }

        var totalItems = await query.CountAsync(ct);
        var items = await query.OrderBy(service => service.Name)
            .ThenBy(service => service.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

        return (items, totalItems);

    }

    public Task<bool> ExistsWithNameExceptId(string name, Guid serviceId, CancellationToken ct = default)
    {
        return _dbContext.Services.AsNoTracking()
            .AnyAsync(service => service.Name.ToLower() == name.ToLower() && service.Id != serviceId, ct);
    }

}
