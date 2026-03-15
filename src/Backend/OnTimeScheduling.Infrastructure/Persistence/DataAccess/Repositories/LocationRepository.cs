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

    public async Task<bool> ExistsActiveLocationWithName(string name, Guid companyId, CancellationToken ct)
    {
        return await _dbContext.Locations.AnyAsync(l => l.Name.ToLower().Equals(name.ToLower())
                                                         && l.CompanyId == companyId, ct);
    }

    public async Task<bool> ExistsActiveLocationById(Guid locationId, CancellationToken ct = default)
    {
        return await _dbContext.Locations
            .AnyAsync(l => l.Id == locationId && l.Status == RecordStatus.Active, ct);
    }

    public async Task<string?> GetActiveLocationTimeZoneIdById(Guid locationId, CancellationToken ct = default)
    {
        return await _dbContext.Locations
            .Where(l => l.Id == locationId && l.Status == RecordStatus.Active)
            .Select(l => l.TimeZoneId)
            .FirstOrDefaultAsync(ct);
    }


}
