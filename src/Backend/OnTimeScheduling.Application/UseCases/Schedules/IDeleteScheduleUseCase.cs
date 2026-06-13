namespace OnTimeScheduling.Application.UseCases.Schedules;

public interface IDeleteScheduleUseCase
{
    Task ExecuteAsync(Guid id, CancellationToken ct = default);
}
