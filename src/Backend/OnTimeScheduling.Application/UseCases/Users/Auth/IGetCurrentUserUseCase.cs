using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Users.Auth;

public interface IGetCurrentUserUseCase
{
    Task<ResponseUserProfileJson> ExecuteAsync(CancellationToken ct = default);
}
