namespace OnTimeScheduling.Application.UseCases.ScheduleBlocks;

public interface IDeleteScheduleBlockUseCase
{
    Task ExecuteAsync(Guid id, CancellationToken ct = default);
}
