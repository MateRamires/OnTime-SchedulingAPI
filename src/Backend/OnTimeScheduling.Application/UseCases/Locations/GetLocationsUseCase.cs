using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Locations;

public class GetLocationsUseCase : IGetLocationsUseCase
{
    private readonly ILocationReadOnlyRepository _locationReadRepository;
    private readonly ITenantProvider _tenantProvider;

    public GetLocationsUseCase(ILocationReadOnlyRepository locationReadRepository, ITenantProvider tenantProvider)
    {
        _locationReadRepository = locationReadRepository;
        _tenantProvider = tenantProvider;
    }

    public async Task<List<ResponseLocationJson>> ExecuteAsync(RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default)
    {
        _ = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var locations = await _locationReadRepository.GetAllAsync(status, searchTerm, ct);

        return locations.Select(LocationResponseMapper.Map).ToList();
    }

}
