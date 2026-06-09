using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.ScheduleBlocks;

public interface IGetScheduleBlocksUseCase
{
    Task<ResponsePagedResultJson<ResponseScheduleBlockJson>> ExecuteAsync(RequestGetScheduleBlocksJson request, CancellationToken ct = default);
}
