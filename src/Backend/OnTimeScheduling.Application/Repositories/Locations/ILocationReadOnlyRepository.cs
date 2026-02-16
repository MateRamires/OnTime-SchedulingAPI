namespace OnTimeScheduling.Application.Repositories.Locations;

public interface ILocationReadOnlyRepository
{
    Task<bool> ExistsActiveLocationWithName(string name, Guid companyId, CancellationToken ct);
}
