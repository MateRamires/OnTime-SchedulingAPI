using OnTimeScheduling.Domain.Entities.User;

namespace OnTimeScheduling.Application.UseCases.Users.CreateUser;

public interface ICreateUserUseCase
{
    public Task<Guid> ExecuteAsync();
}
