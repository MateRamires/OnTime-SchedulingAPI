using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Domain.Entities.Locations;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;

public class LocationRepository : ILocationWriteOnlyRepository, ILocationReadOnlyRepository
{
    private readonly AppDbContext _dbContext;

    public LocationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Location location, CancellationToken cancellationToken)
    {
        await _dbContext.Locations.AddAsync(location, cancellationToken);
    }
    public void Update(Location location)
    {
        _dbContext.Locations.Update(location);
    }
    public async Task<bool> ExistsActiveLocationWithName(string name, Guid companyId, CancellationToken ct)
    {
        return await _dbContext.Locations.AsNoTracking().AnyAsync(l => l.Name.ToLower().Equals(name.ToLower())
                                                             && l.CompanyId == companyId
                                                             && l.Status == RecordStatus.Active, ct);

    }
    public async Task<bool> ExistsLocationWithName(string name, Guid companyId, CancellationToken ct = default)
    {
        return await _dbContext.Locations.AsNoTracking().AnyAsync(l => l.Name.ToLower().Equals(name.ToLower())
                                                             && l.CompanyId == companyId, ct);
    }

    public async Task<bool> ExistsLocationWithNameExceptId(string name, Guid locationId, Guid companyId, CancellationToken ct = default)
    {
        return await _dbContext.Locations.AsNoTracking().AnyAsync(l => l.Name.ToLower().Equals(name.ToLower())
                                                             && l.CompanyId == companyId
                                                             && l.Id != locationId, ct);
    }


    public async Task<bool> ExistsActiveLocationById(Guid locationId, CancellationToken ct = default)
    {
        return await _dbContext.Locations
            .AsNoTracking()
            .AnyAsync(l => l.Id == locationId && l.Status == RecordStatus.Active, ct);
    }

    public async Task<string?> GetActiveLocationTimeZoneIdById(Guid locationId, CancellationToken ct = default)
    {
        return await _dbContext.Locations
            .AsNoTracking()
            .Where(l => l.Id == locationId && l.Status == RecordStatus.Active)
            .Select(l => l.TimeZoneId)
            .FirstOrDefaultAsync(ct);
    }
    public Task<Location?> GetByIdAsync(Guid locationId, CancellationToken ct = default)
    {
        return _dbContext.Locations
            .FirstOrDefaultAsync(l => l.Id == locationId, ct);
    }

    public Task<List<Location>> GetAllAsync(RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default)
    {
        var query = _dbContext.Locations.AsNoTracking().AsQueryable();

        if (status.HasValue)
            query = query.Where(location => location.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.Trim().ToLower();
            query = query.Where(location =>
                location.Name.ToLower().Contains(normalizedSearchTerm) ||
                location.Address.ToLower().Contains(normalizedSearchTerm));
        }

        return query
            .OrderBy(location => location.Name)
            .ToListAsync(ct);
    }


}
