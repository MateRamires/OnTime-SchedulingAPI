namespace OnTimeScheduling.Application.UseCases.Services;

public interface IUnlinkProfessionalServiceUseCase
{
    Task ExecuteAsync(Guid serviceId, Guid professionalId, CancellationToken ct = default);
}
