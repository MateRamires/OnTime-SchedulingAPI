using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.UseCases.ScheduleBlocks;

public interface IUpdateScheduleBlockUseCase
{
    Task ExecuteAsync(Guid id, RequestUpdateScheduleBlockJson request, CancellationToken ct = default);
}
