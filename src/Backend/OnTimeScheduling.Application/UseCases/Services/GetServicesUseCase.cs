using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.UseCases.Services.Mapper;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Services;

public class GetServicesUseCase : IGetServicesUseCase
{
    private readonly IServiceReadOnlyRepository _serviceReadOnlyRepository;
    private readonly ITenantProvider _tenantProvider;

    public GetServicesUseCase(IServiceReadOnlyRepository serviceReadOnlyRepository, ITenantProvider tenantProvider)
    {
        _serviceReadOnlyRepository = serviceReadOnlyRepository;
        _tenantProvider = tenantProvider;
    }

    public async Task<List<ResponseServiceJson>> ExecuteAsync(RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default)
    {
        _ = _tenantProvider.CompanyId ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var services = await _serviceReadOnlyRepository.GetAllAsync(status, searchTerm, ct);
        return services.Select(ServiceResponseMapper.Map).ToList();
    }

}
