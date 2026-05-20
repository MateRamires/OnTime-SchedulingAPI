namespace OnTimeScheduling.Application.UseCases.Users.Management;

public interface IInactivateUserUseCase
{
    Task ExecuteAsync(Guid userId, CancellationToken ct = default);
}
