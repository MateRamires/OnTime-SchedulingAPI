using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Locations;

public class InactivateLocationUseCase : IInactivateLocationUseCase
{
    private readonly ILocationReadOnlyRepository _locationReadRepository;
    private readonly ILocationWriteOnlyRepository _locationWriteRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public InactivateLocationUseCase(
        ILocationReadOnlyRepository locationReadRepository,
        ILocationWriteOnlyRepository locationWriteRepository,
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork)
    {
        _locationReadRepository = locationReadRepository;
        _locationWriteRepository = locationWriteRepository;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid locationId, CancellationToken ct = default)
    {
        _ = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var location = await _locationReadRepository.GetByIdAsync(locationId, ct)
            ?? throw new NotFoundException("Location not found.");

        location.Inactivate();

        _locationWriteRepository.Update(location);
        await _unitOfWork.Commit(ct);
    }

}
