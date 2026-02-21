using OnTimeScheduling.Domain.Entities.Services;

namespace OnTimeScheduling.Application.Repositories.Services;

public interface IServiceWriteOnlyRepository
{
    Task Add(Service service, CancellationToken ct = default);
}
