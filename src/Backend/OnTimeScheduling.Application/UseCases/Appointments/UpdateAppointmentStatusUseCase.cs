using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Communication.Enums;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public class UpdateAppointmentStatusUseCase : IUpdateAppointmentStatusUseCase
{
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly IAppointmentWriteOnlyRepository _appointmentWriteRepository;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAppointmentStatusUseCase(
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

    public async Task ExecuteAsync(Guid appointmentId, RequestUpdateProviderAppointmentStatusJson request, CancellationToken ct = default)
    {
        var loggedUser = _loggedUser.GetUser();

        var appointment = await _appointmentReadRepository.GetAppointmentByIdAsync(appointmentId, ct)
            ?? throw new NotFoundException("Appointment not found.");

        if (appointment.ProfessionalId != loggedUser.Id)
            throw new ErrorOnUnauthorizedException("Providers can only update their own appointments.");

        switch (request.Status)
        {
            case AppointmentOutcomeStatus.COMPLETED:
                appointment.MarkAsCompleted();
                break;
            case AppointmentOutcomeStatus.NO_SHOW:
                appointment.MarkAsNoShow();
                break;
            default:
                throw new ErrorOnValidationException(["Invalid status for provider outcome update."]);
        }

        _appointmentWriteRepository.Update(appointment);
        await _unitOfWork.Commit(ct);
    }

}
