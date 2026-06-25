using OnTimeScheduling.Application.Repositories.Schedules;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Schedules;

public class DeleteScheduleUseCase : IDeleteScheduleUseCase
{
    private readonly IProfessionalScheduleReadOnlyRepository _readRepository;
    private readonly IProfessionalScheduleWriteOnlyRepository _writeRepository;
    private readonly FutureAppointmentScheduleGuard _futureAppointmentScheduleGuard;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteScheduleUseCase(
        IProfessionalScheduleReadOnlyRepository readRepository,
        IProfessionalScheduleWriteOnlyRepository writeRepository,
        FutureAppointmentScheduleGuard futureAppointmentScheduleGuard,
        IUnitOfWork unitOfWork)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _futureAppointmentScheduleGuard = futureAppointmentScheduleGuard;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var schedule = await _readRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Professional schedule not found.");

        await _futureAppointmentScheduleGuard.EnsureCanDeleteAsync(schedule, ct);

        _writeRepository.Delete(schedule);
        await _unitOfWork.Commit(ct);
    }
}
