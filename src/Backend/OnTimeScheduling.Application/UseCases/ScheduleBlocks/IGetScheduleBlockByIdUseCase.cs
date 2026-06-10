using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.ScheduleBlocks;

public interface IGetScheduleBlockByIdUseCase
{
    Task<ResponseScheduleBlockJson> ExecuteAsync(Guid id, CancellationToken ct = default);
}
