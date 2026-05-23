using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.UseCases.Services;

public interface IGetServicesUseCase
{
    Task<List<ResponseServiceJson>> ExecuteAsync(RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default);
}
