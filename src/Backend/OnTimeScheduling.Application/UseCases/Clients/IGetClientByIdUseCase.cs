using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Clients;

public interface IGetClientByIdUseCase
{
    Task<ResponseClientJson> ExecuteAsync(Guid clientId, CancellationToken ct = default);
}
