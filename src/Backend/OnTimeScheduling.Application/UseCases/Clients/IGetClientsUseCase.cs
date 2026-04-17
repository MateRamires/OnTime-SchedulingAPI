using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Clients;

public interface IGetClientsUseCase
{
    Task<List<ResponseClientJson>> ExecuteAsync(CancellationToken ct = default);
}
