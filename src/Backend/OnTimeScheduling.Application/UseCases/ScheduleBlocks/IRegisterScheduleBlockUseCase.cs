namespace OnTimeScheduling.Application.UseCases.ScheduleBlocks;

public interface IRegisterScheduleBlockUseCase
{
    Task<ResponseRegisterScheduleBlockJson> ExecuteAsync(RequestRegisterScheduleBlockJson request, CancellationToken ct = default);
}
