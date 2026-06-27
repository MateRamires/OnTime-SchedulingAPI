using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.UseCases.Clients;

public interface IGetClientsUseCase
{
    Task<ResponsePagedResultJson<ResponseClientJson>> ExecuteAsync(
        RequestPaginationQuery pagination,
        RecordStatus? status = null,
        string? searchTerm = null,
        CancellationToken ct = default);
}
