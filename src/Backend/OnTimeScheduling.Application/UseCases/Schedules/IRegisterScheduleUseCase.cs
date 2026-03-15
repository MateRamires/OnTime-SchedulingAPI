using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Schedules;

public interface IRegisterScheduleUseCase
{
    Task<ResponseRegisterScheduleJson> ExecuteAsync(RequestRegisterScheduleJson request, CancellationToken ct = default);
}
