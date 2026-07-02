using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Security.Concurrency;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public class CancelAppointmentUseCase : ICancelAppointmentUseCase
{
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly IAppointmentWriteOnlyRepository _appointmentWriteRepository;
    private readonly IAgendaConcurrencyGuard _agendaConcurrencyGuard;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;

    public CancelAppointmentUseCase(
        IAppointmentReadOnlyRepository appointmentReadRepository,
        IAppointmentWriteOnlyRepository appointmentWriteRepository,
        IAgendaConcurrencyGuard agendaConcurrencyGuard,
        ILoggedUser loggedUser,
        IUnitOfWork unitOfWork)
    {
        _appointmentReadRepository = appointmentReadRepository;
        _appointmentWriteRepository = appointmentWriteRepository;
        _agendaConcurrencyGuard = agendaConcurrencyGuard;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid appointmentId, CancellationToken ct = default)
    {
        var loggedUser = _loggedUser.GetUser();

        if (loggedUser.Role is not UserRole.COMPANY_ADMIN and not UserRole.ATTENDANT)
            throw new ErrorOnUnauthorizedException("Only company administrators and attendants can cancel appointments.");

        await _agendaConcurrencyGuard.ExecuteAsync(
            [AgendaConcurrencyLockKey.ForAppointment(appointmentId)],
            async lockedCt =>
            {
                var appointment = await _appointmentReadRepository.GetAppointmentByIdAsync(appointmentId, lockedCt)
                    ?? throw new NotFoundException("Appointment not found.");

                await _agendaConcurrencyGuard.ExecuteAsync(
                    [
                        AgendaConcurrencyLockKey.ForProfessional(appointment.ProfessionalId),
                        AgendaConcurrencyLockKey.ForLocation(appointment.LocationId)
                    ],
                    async appointmentScopeLockedCt =>
                    {
                        appointment.Cancel();

                        _appointmentWriteRepository.Update(appointment);
                        await _unitOfWork.Commit(appointmentScopeLockedCt);
                    },
                    lockedCt);
            },
            ct);
    }
}
