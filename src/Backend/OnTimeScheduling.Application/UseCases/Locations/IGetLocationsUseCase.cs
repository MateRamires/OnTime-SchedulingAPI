using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.UseCases.Locations;

public interface IGetLocationsUseCase
{
    Task<List<ResponseLocationJson>> ExecuteAsync(RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default);
}
