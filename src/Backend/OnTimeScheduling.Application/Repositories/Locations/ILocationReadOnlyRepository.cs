using OnTimeScheduling.Domain.Entities.Locations;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.Repositories.Locations;

public interface ILocationReadOnlyRepository
{
    Task<bool> ExistsActiveLocationWithName(string name, Guid companyId, CancellationToken ct);
    Task<bool> ExistsLocationWithName(string name, Guid companyId, CancellationToken ct = default);
    Task<bool> ExistsLocationWithNameExceptId(string name, Guid locationId, Guid companyId, CancellationToken ct = default);
    Task<bool> ExistsActiveLocationById(Guid locationId, CancellationToken ct = default);
    Task<string?> GetActiveLocationTimeZoneIdById(Guid locationId, CancellationToken ct = default);
    Task<Location?> GetByIdAsync(Guid locationId, CancellationToken ct = default);
    Task<List<Location>> GetAllAsync(RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default);

}
