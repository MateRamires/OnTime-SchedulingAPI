using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Domain.Entities.Locations;

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
}
