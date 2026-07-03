using OnTimeScheduling.Application.Repositories.Schedules;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Security.Concurrency;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Schedules;

public class DeleteScheduleUseCase : IDeleteScheduleUseCase
{
    private readonly IProfessionalScheduleReadOnlyRepository _readRepository;
    private readonly IProfessionalScheduleWriteOnlyRepository _writeRepository;
    private readonly FutureAppointmentScheduleGuard _futureAppointmentScheduleGuard;
    private readonly IAgendaConcurrencyGuard _agendaConcurrencyGuard;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteScheduleUseCase(
        IProfessionalScheduleReadOnlyRepository readRepository,
        IProfessionalScheduleWriteOnlyRepository writeRepository,
        FutureAppointmentScheduleGuard futureAppointmentScheduleGuard,
        IAgendaConcurrencyGuard agendaConcurrencyGuard,
        IUnitOfWork unitOfWork)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _futureAppointmentScheduleGuard = futureAppointmentScheduleGuard;
        _agendaConcurrencyGuard = agendaConcurrencyGuard;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        await _agendaConcurrencyGuard.ExecuteAsync(
            [AgendaConcurrencyLockKey.ForProfessionalSchedule(id)],
            async lockedCt =>
            {
                var schedule = await _readRepository.GetByIdAsync(id, lockedCt)
                    ?? throw new NotFoundException("Professional schedule not found.");

                await _agendaConcurrencyGuard.ExecuteAsync(
                    [
                        AgendaConcurrencyLockKey.ForProfessional(schedule.UserId),
                        AgendaConcurrencyLockKey.ForLocation(schedule.LocationId)
                    ],
                    async scheduleLockedCt =>
                    {
                        await _futureAppointmentScheduleGuard.EnsureCanDeleteAsync(schedule, scheduleLockedCt);

                        _writeRepository.Delete(schedule);
                        await _unitOfWork.Commit(scheduleLockedCt);
                    },
                    lockedCt);
            },
            ct);
    }
}
