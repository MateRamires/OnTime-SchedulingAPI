using OnTimeScheduling.Domain.Entities.Locations;

namespace OnTimeScheduling.Application.Repositories.Locations;

public interface ILocationWriteOnlyRepository
{
    Task Add(Location location, CancellationToken cancellationToken);
}
