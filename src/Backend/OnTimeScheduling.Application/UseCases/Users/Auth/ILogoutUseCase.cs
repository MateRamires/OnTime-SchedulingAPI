namespace OnTimeScheduling.Application.UseCases.Users.Auth;

public interface ILogoutUseCase
{
    Task ExecuteAsync(RequestLogoutJson request, CancellationToken ct = default);
}
