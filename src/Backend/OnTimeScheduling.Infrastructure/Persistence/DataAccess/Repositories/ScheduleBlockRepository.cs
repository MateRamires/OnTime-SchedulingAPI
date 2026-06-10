using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Repositories.ScheduleBlocks;
using OnTimeScheduling.Domain.Entities.ScheduleBlocks;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;

public class ScheduleBlockRepository : IScheduleBlockReadOnlyRepository, IScheduleBlockWriteOnlyRepository
{
    private readonly AppDbContext _dbContext;

    public ScheduleBlockRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(ScheduleBlock block, CancellationToken ct = default)
    {
        await _dbContext.ScheduleBlocks.AddAsync(block, ct);
    }

    public void Update(ScheduleBlock block)
    {
        _dbContext.ScheduleBlocks.Update(block);
    }

    public void Delete(ScheduleBlock block)
    {
        _dbContext.ScheduleBlocks.Remove(block);
    }

    public async Task<ScheduleBlock?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.ScheduleBlocks.FirstOrDefaultAsync(b => b.Id == id, ct);
    }

    public async Task<ScheduleBlockDetails?> GetDetailsByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await BuildDetailsQuery().FirstOrDefaultAsync(b => b.Block.Id == id, ct);
    }

    public async Task<(List<ScheduleBlockDetails> Items, int TotalItems)> GetAllAsync(
        int skip,
        int take,
        Guid? professionalId,
        Guid? locationId,
        DateTime? startTimeUtc,
        DateTime? endTimeUtc,
        bool includeExpired,
        CancellationToken ct = default)
    {
        var query = BuildDetailsQuery();

        if (professionalId.HasValue)
            query = query.Where(b => b.Block.ProfessionalId == professionalId.Value);

        if (locationId.HasValue)
            query = query.Where(b => b.Block.LocationId == locationId.Value);

        if (startTimeUtc.HasValue)
            query = query.Where(b => b.Block.EndTime > startTimeUtc.Value);

        if (endTimeUtc.HasValue)
            query = query.Where(b => b.Block.StartTime < endTimeUtc.Value);

        if (!includeExpired)
        {
            var nowUtc = DateTime.UtcNow;
            query = query.Where(b => b.Block.EndTime > nowUtc);
        }

        var totalItems = await query.CountAsync(ct);

        var items = await query
            .OrderBy(b => b.Block.StartTime)
            .ThenBy(b => b.Block.EndTime)
            .ThenBy(b => b.Block.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

        return (items, totalItems);
    }

    public async Task<bool> HasOverlappingBlockForAppointmentAsync(
        Guid professionalId,
        Guid locationId,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        CancellationToken ct = default,
        Guid? ignoredBlockId = null)
    {
        return await _dbContext.ScheduleBlocks
            .AsNoTracking()
            .AnyAsync(b =>
                (!ignoredBlockId.HasValue || b.Id != ignoredBlockId.Value) &&
                b.StartTime < endTimeUtc &&
                b.EndTime > startTimeUtc &&
                (!b.ProfessionalId.HasValue || b.ProfessionalId == professionalId) &&
                (!b.LocationId.HasValue || b.LocationId == locationId),
                ct);
    }

    public async Task<List<ScheduleBlock>> GetOverlappingBlocksForAppointmentAsync(
        Guid professionalId,
        Guid locationId,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        CancellationToken ct = default)
    {
        return await _dbContext.ScheduleBlocks
            .AsNoTracking()
            .Where(b =>
                b.StartTime < endTimeUtc &&
                b.EndTime > startTimeUtc &&
                (!b.ProfessionalId.HasValue || b.ProfessionalId == professionalId) &&
                (!b.LocationId.HasValue || b.LocationId == locationId))
            .OrderBy(b => b.StartTime)
            .ToListAsync(ct);
    }

    private IQueryable<ScheduleBlockDetails> BuildDetailsQuery()
    {
        return _dbContext.ScheduleBlocks
            .AsNoTracking()
            .GroupJoin(_dbContext.Users.AsNoTracking(),
                block => block.ProfessionalId,
                professional => (Guid?)professional.Id,
                (block, professionals) => new { block, professionals })
            .SelectMany(x => x.professionals.DefaultIfEmpty(),
                (x, professional) => new { x.block, professional })
            .GroupJoin(_dbContext.Locations.AsNoTracking(),
                x => x.block.LocationId,
                location => (Guid?)location.Id,
                (x, locations) => new { x.block, x.professional, locations })
            .SelectMany(x => x.locations.DefaultIfEmpty(),
                (x, location) => new ScheduleBlockDetails
                {
                    Block = x.block,
                    ProfessionalName = x.professional == null ? null : x.professional.Name,
                    LocationName = location == null ? null : location.Name
                });
    }

}
