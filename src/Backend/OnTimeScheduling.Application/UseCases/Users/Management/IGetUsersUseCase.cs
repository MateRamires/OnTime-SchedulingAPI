using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.UseCases.Users.Management;

public interface IGetUsersUseCase
{
    Task<ResponsePagedResultJson<ResponseUserJson>> ExecuteAsync(RequestPaginationQuery pagination, UserRole? role = null, RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default);
}
