using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.UseCases.Clients;

public interface IUpdateClientUseCase
{
    Task ExecuteAsync(Guid clientId, RequestUpdateClientJson request, CancellationToken ct = default);
}
