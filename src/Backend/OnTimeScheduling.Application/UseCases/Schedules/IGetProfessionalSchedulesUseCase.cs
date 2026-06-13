using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Schedules;

public interface IGetProfessionalSchedulesUseCase
{
    Task<ResponsePagedResultJson<ResponseProfessionalScheduleJson>> ExecuteAsync(RequestGetProfessionalSchedulesJson request, CancellationToken ct = default);
}
