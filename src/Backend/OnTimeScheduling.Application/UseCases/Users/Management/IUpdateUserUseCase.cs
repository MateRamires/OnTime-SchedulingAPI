namespace OnTimeScheduling.Application.UseCases.Users.Management;

public interface IUpdateUserUseCase
{
    Task ExecuteAsync(Guid userId, RequestUpdateUserJson request, CancellationToken ct = default);
}
