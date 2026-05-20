using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Users.Management;

public interface IGetUserByIdUseCase
{
    Task<ResponseUserJson> ExecuteAsync(Guid userId, CancellationToken ct = default);
}
