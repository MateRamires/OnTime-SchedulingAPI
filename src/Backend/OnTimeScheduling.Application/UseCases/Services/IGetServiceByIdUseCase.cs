using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Services;

public interface IGetServiceByIdUseCase
{
    Task<ResponseServiceJson> ExecuteAsync(Guid serviceId, CancellationToken ct = default);
}
