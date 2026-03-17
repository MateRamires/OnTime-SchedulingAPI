using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public interface IGetAvailableTimeSlotsUseCase
{
    Task<ResponseAvailableTimeSlotsJson> ExecuteAsync(RequestGetAvailableTimeSlotsJson request, CancellationToken ct = default);
}
