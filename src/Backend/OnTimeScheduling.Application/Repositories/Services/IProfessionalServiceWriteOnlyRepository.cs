using OnTimeScheduling.Domain.Entities.Services;

namespace OnTimeScheduling.Application.Repositories.Services;

public interface IProfessionalServiceWriteOnlyRepository
{
    Task Add(ProfessionalService professionalService, CancellationToken ct = default);
    Task Delete(Guid userId, Guid serviceId, CancellationToken ct = default);
}
