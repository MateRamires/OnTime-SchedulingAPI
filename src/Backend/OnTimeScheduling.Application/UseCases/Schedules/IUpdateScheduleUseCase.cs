using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.UseCases.Schedules;

public interface IUpdateScheduleUseCase
{
    Task ExecuteAsync(Guid id, RequestUpdateScheduleJson request, CancellationToken ct = default);
}
