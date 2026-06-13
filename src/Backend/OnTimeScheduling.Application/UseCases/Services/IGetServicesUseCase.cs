using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.UseCases.Services;

public interface IGetServicesUseCase
{
    Task<ResponsePagedResultJson<ResponseServiceJson>> ExecuteAsync(RequestPaginationQuery pagination, RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default);
}
