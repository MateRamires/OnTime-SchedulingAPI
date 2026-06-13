using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Schedules;

public interface IGetProfessionalScheduleByIdUseCase
{
    Task<ResponseProfessionalScheduleJson> ExecuteAsync(Guid id, CancellationToken ct = default);
}
