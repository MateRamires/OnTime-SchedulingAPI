using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.UseCases.Users.Management;

public interface IGetUsersUseCase
{
    Task<List<ResponseUserJson>> ExecuteAsync(UserRole? role = null, RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default);
}
