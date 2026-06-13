using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Users.Auth;

public interface IRefreshTokenUseCase
{
    Task<ResponseLoginJson> ExecuteAsync(RequestRefreshTokenJson request, CancellationToken ct = default);
}
