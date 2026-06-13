namespace OnTimeScheduling.Application.UseCases.Users.Management;

public interface IActivateUserUseCase
{
    Task ExecuteAsync(Guid userId, CancellationToken ct = default);
}
