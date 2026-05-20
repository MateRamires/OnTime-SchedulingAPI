using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.UseCases.Locations.Mapper;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Locations;

public class GetLocationByIdUseCase : IGetLocationByIdUseCase
{
    private readonly ILocationReadOnlyRepository _locationReadRepository;
    private readonly ITenantProvider _tenantProvider;

    public GetLocationByIdUseCase(ILocationReadOnlyRepository locationReadRepository, ITenantProvider tenantProvider)
    {
        _locationReadRepository = locationReadRepository;
        _tenantProvider = tenantProvider;
    }

    public async Task<ResponseLocationJson> ExecuteAsync(Guid locationId, CancellationToken ct = default)
    {
        _ = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var location = await _locationReadRepository.GetByIdAsync(locationId, ct)
            ?? throw new NotFoundException("Location not found.");

        return LocationResponseMapper.Map(location);
    }

}
