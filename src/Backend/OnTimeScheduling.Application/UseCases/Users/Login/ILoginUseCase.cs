using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Users.Login;

public interface ILoginUseCase
{
    public Task<ResponseLoginJson> ExecuteAsync(RequestLoginJson request);
}
