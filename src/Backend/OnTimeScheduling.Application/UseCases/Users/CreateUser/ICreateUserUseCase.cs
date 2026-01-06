using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.UseCases.Users.CreateUser;

public interface ICreateUserUseCase
{
    public Task<Guid> ExecuteAsync(RequestRegisterUserJson request, CancellationToken ct = default);
}
