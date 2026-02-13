using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Domain.Entities.Locations;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;

public class LocationRepository : ILocationWriteOnlyRepository
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
}
