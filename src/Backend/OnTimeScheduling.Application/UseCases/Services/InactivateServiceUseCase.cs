using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Services;

public class InactivateServiceUseCase : IInactivateServiceUseCase
{
    private readonly IServiceReadOnlyRepository _serviceReadOnlyRepository;
    private readonly IServiceWriteOnlyRepository _serviceWriteOnlyRepository;
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InactivateServiceUseCase(
        IServiceReadOnlyRepository serviceReadOnlyRepository,
        IServiceWriteOnlyRepository serviceWriteOnlyRepository,
        IAppointmentReadOnlyRepository appointmentReadRepository,
        IUnitOfWork unitOfWork)
    {
        _serviceReadOnlyRepository = serviceReadOnlyRepository;
        _serviceWriteOnlyRepository = serviceWriteOnlyRepository;
        _appointmentReadRepository = appointmentReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid serviceId, CancellationToken ct = default)
    {
        var service = await _serviceReadOnlyRepository.GetByIdAsync(serviceId, ct)
            ?? throw new NotFoundException("Service not found.");

        var hasFutureAppointments = await _appointmentReadRepository
            .HasFutureScheduledAppointmentsAsync(serviceId: serviceId, ct: ct);

        if (hasFutureAppointments)
            throw new ConflictException("Cannot inactivate a service with future scheduled appointments. Cancel or reschedule those appointments first.");

        service.Inactivate();
        _serviceWriteOnlyRepository.Update(service);
        await _unitOfWork.Commit(ct);
    }
}
