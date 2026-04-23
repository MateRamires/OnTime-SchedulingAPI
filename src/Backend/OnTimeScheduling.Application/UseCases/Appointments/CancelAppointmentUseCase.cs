
using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public class CancelAppointmentUseCase : ICancelAppointmentUseCase
{
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly IAppointmentWriteOnlyRepository _appointmentWriteRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;

    public CancelAppointmentUseCase(
        IAppointmentReadOnlyRepository appointmentReadRepository,
        IAppointmentWriteOnlyRepository appointmentWriteRepository,
        ILoggedUser loggedUser,
        IUnitOfWork unitOfWork)
    {
        _appointmentReadRepository = appointmentReadRepository;
        _appointmentWriteRepository = appointmentWriteRepository;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid appointmentId, CancellationToken ct = default)
    {
        var loggedUser = _loggedUser.GetUser();

        var appointment = await _appointmentReadRepository.GetAppointmentByIdAsync(appointmentId, ct)
            ?? throw new NotFoundException("Appointment not found.");

        if (loggedUser.Role == UserRole.PROVIDER && appointment.ProfessionalId != loggedUser.Id)
            throw new ErrorOnUnauthorizedException("Providers can only cancel their own appointments.");

        appointment.Cancel();

        _appointmentWriteRepository.Update(appointment);
        await _unitOfWork.Commit(ct);
    }
}
