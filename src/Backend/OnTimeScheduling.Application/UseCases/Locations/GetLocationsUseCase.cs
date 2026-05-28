using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.UseCases.Locations.Mapper;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
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

    public async Task<ResponsePagedResultJson<ResponseLocationJson>> ExecuteAsync(RequestPaginationQuery pagination, RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default)
    {
        _ = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var (locations, totalItems) = await _locationReadRepository.GetAllAsync(pagination.Skip, pagination.Size, status, searchTerm, ct);
        var items = locations.Select(LocationResponseMapper.Map).ToList();

        return new ResponsePagedResultJson<ResponseLocationJson>
        {
            Page = pagination.Page,
            Size = pagination.Size,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pagination.Size),
            Items = items
        };

    }

}
