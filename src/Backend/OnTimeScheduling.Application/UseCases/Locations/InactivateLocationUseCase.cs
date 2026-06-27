using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Locations;

public class InactivateLocationUseCase : IInactivateLocationUseCase
{
    private readonly ILocationReadOnlyRepository _locationReadRepository;
    private readonly ILocationWriteOnlyRepository _locationWriteRepository;
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUnitOfWork _unitOfWork;

    public InactivateLocationUseCase(
        ILocationReadOnlyRepository locationReadRepository,
        ILocationWriteOnlyRepository locationWriteRepository,
        IAppointmentReadOnlyRepository appointmentReadRepository,
        ITenantProvider tenantProvider,
        IUnitOfWork unitOfWork)
    {
        _locationReadRepository = locationReadRepository;
        _locationWriteRepository = locationWriteRepository;
        _appointmentReadRepository = appointmentReadRepository;
        _tenantProvider = tenantProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid locationId, CancellationToken ct = default)
    {
        _ = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var location = await _locationReadRepository.GetByIdAsync(locationId, ct)
            ?? throw new NotFoundException("Location not found.");

        var hasFutureAppointments = await _appointmentReadRepository
            .HasFutureScheduledAppointmentsAsync(locationId: locationId, ct: ct);

        if (hasFutureAppointments)
            throw new ConflictException("Cannot inactivate a location with future scheduled appointments. Cancel or reschedule those appointments first.");

        location.Inactivate();

        _locationWriteRepository.Update(location);
        await _unitOfWork.Commit(ct);
    }
}
