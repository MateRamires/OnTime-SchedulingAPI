using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Services;

public class UnlinkProfessionalServiceUseCase : IUnlinkProfessionalServiceUseCase
{
    private readonly IProfessionalServiceReadOnlyRepository _professionalServiceReadOnlyRepository;
    private readonly IProfessionalServiceWriteOnlyRepository _professionalServiceWriteOnlyRepository;
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnlinkProfessionalServiceUseCase(
        IProfessionalServiceReadOnlyRepository professionalServiceReadOnlyRepository,
        IProfessionalServiceWriteOnlyRepository professionalServiceWriteOnlyRepository,
        IAppointmentReadOnlyRepository appointmentReadRepository,
        IUnitOfWork unitOfWork)
    {
        _professionalServiceReadOnlyRepository = professionalServiceReadOnlyRepository;
        _professionalServiceWriteOnlyRepository = professionalServiceWriteOnlyRepository;
        _appointmentReadRepository = appointmentReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid serviceId, Guid professionalId, CancellationToken ct = default)
    {
        var linked = await _professionalServiceReadOnlyRepository.Exists(professionalId, serviceId, ct);
        if (!linked)
            throw new NotFoundException("Service-professional link not found.");

        var hasFutureAppointments = await _appointmentReadRepository
            .HasFutureScheduledAppointmentsAsync(
                professionalId: professionalId,
                serviceId: serviceId,
                ct: ct);

        if (hasFutureAppointments)
            throw new ConflictException("Cannot unlink a professional from a service with future scheduled appointments. Cancel or reschedule those appointments first.");

        await _professionalServiceWriteOnlyRepository.Delete(professionalId, serviceId, ct);
        await _unitOfWork.Commit(ct);
    }
}
