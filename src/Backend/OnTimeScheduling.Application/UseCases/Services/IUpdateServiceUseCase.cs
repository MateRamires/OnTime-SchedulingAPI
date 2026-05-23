using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.UseCases.Services;

public interface IUpdateServiceUseCase
{
    Task ExecuteAsync(Guid serviceId, RequestUpdateServiceJson request, CancellationToken ct = default);
}
