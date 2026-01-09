using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Users.CreateUser;

public interface ICreateUserUseCase
{
    public Task<ResponseRegisteredUserJson> ExecuteAsync(RequestRegisterUserJson request, CancellationToken ct = default);
}
