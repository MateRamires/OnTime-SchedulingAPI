using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Domain.Entities.Services;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;

public class ProfessionalServiceRepository : IProfessionalServiceWriteOnlyRepository, IProfessionalServiceReadOnlyRepository
{
    private readonly AppDbContext _dbContext;

    public ProfessionalServiceRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(ProfessionalService professionalService, CancellationToken ct = default)
    {
        await _dbContext.ProfessionalServices.AddAsync(professionalService, ct);
    }

    public async Task Delete(Guid userId, Guid serviceId, CancellationToken ct = default)
    {
        var entity = await _dbContext.ProfessionalServices
            .FirstOrDefaultAsync(ps => ps.UserId == userId && ps.ServiceId == serviceId, ct);

        if (entity is not null)
        {
            _dbContext.ProfessionalServices.Remove(entity);
        }
    }

    public async Task<bool> Exists(Guid userId, Guid serviceId, CancellationToken ct = default)
    {
        return await _dbContext.ProfessionalServices
            .AnyAsync(ps => ps.UserId == userId && ps.ServiceId == serviceId, ct);
    }
}
