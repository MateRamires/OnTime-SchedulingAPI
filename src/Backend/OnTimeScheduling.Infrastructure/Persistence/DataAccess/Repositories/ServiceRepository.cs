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

    public async Task Add(Service service, CancellationToken ct = default)
    {
        await _dbContext.Services.AddAsync(service, ct);
    }

    public async Task<bool> ExistsActiveWithName(string name, CancellationToken ct = default)
    {
        return await _dbContext.Services
            .AnyAsync(s => s.Name.ToLower() == name.ToLower() && s.Status == RecordStatus.Active, ct);
    }
}
