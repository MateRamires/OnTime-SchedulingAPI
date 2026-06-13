using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Clients;

public interface IGetClientsUseCase
{
    Task<ResponsePagedResultJson<ResponseClientJson>> ExecuteAsync(RequestPaginationQuery pagination, CancellationToken ct = default);
}
