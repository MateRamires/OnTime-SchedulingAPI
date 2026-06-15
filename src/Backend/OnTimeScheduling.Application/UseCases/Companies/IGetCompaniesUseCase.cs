using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.UseCases.Companies;

public interface IGetCompaniesUseCase
{
    Task<ResponsePagedResultJson<ResponseCompanyJson>> ExecuteAsync(RequestPaginationQuery pagination, RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default);
}
