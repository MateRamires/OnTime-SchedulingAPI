
using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public class CancelAppointmentUseCase : ICancelAppointmentUseCase
{
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly IAppointmentWriteOnlyRepository _appointmentWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelAppointmentUseCase(
        IAppointmentReadOnlyRepository appointmentReadRepository,
        IAppointmentWriteOnlyRepository appointmentWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentReadRepository = appointmentReadRepository;
        _appointmentWriteRepository = appointmentWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid appointmentId, CancellationToken ct = default)
    {
        var appointment = await _appointmentReadRepository.GetAppointmentByIdAsync(appointmentId, ct)
            ?? throw new NotFoundException("Appointment not found.");

        appointment.Cancel();

        _appointmentWriteRepository.Update(appointment);
        await _unitOfWork.Commit(ct);
    }
}
