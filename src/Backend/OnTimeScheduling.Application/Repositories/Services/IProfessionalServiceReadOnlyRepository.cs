namespace OnTimeScheduling.Application.Repositories.Services;

public interface IProfessionalServiceReadOnlyRepository
{
    Task<bool> Exists(Guid userId, Guid serviceId, CancellationToken ct = default);
}
