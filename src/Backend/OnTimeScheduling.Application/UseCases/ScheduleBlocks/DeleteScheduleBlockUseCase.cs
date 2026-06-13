using OnTimeScheduling.Application.Repositories.ScheduleBlocks;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.ScheduleBlocks;

public class DeleteScheduleBlockUseCase : IDeleteScheduleBlockUseCase
{
    private readonly IScheduleBlockReadOnlyRepository _readRepository;
    private readonly IScheduleBlockWriteOnlyRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteScheduleBlockUseCase(
        IScheduleBlockReadOnlyRepository readRepository,
        IScheduleBlockWriteOnlyRepository writeRepository,
        IUnitOfWork unitOfWork)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var block = await _readRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Schedule block not found.");

        _writeRepository.Delete(block);
        await _unitOfWork.Commit(ct);
    }

}
