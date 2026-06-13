using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Repositories.Schedules;
using OnTimeScheduling.Domain.Entities.Schedules;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;

public class ProfessionalScheduleRepository : IProfessionalScheduleWriteOnlyRepository, IProfessionalScheduleReadOnlyRepository
{
    private readonly AppDbContext _dbContext;

    public ProfessionalScheduleRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(ProfessionalSchedule schedule, CancellationToken ct = default)
    {
        await _dbContext.ProfessionalSchedules.AddAsync(schedule, ct);
    }

    public void Update(ProfessionalSchedule schedule)
    {
        _dbContext.ProfessionalSchedules.Update(schedule);
    }

    public void Delete(ProfessionalSchedule schedule)
    {
        _dbContext.ProfessionalSchedules.Remove(schedule);
    }

    public async Task<ProfessionalSchedule?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.ProfessionalSchedules.FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<ProfessionalScheduleDetails?> GetDetailsByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await BuildDetailsQuery().FirstOrDefaultAsync(s => s.Schedule.Id == id, ct);
    }

    public async Task<(List<ProfessionalScheduleDetails> Items, int TotalItems)> GetAllAsync(
        int skip,
        int take,
        Guid? professionalId,
        Guid? locationId,
        DayOfWeek? dayOfWeek,
        CancellationToken ct = default)
    {
        var query = BuildDetailsQuery();

        if (professionalId.HasValue)
            query = query.Where(s => s.Schedule.UserId == professionalId.Value);

        if (locationId.HasValue)
            query = query.Where(s => s.Schedule.LocationId == locationId.Value);

        if (dayOfWeek.HasValue)
            query = query.Where(s => s.Schedule.DayOfWeek == dayOfWeek.Value);

        var totalItems = await query.CountAsync(ct);

        var items = await query
            .OrderBy(s => s.ProfessionalName)
            .ThenBy(s => s.LocationName)
            .ThenBy(s => s.Schedule.DayOfWeek)
            .ThenBy(s => s.Schedule.StartTime)
            .ThenBy(s => s.Schedule.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

        return (items, totalItems);
    }

    public async Task<bool> HasOverlappingSchedule(
        Guid userId,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        CancellationToken ct = default,
        Guid? ignoredScheduleId = null)
    {
        return await _dbContext.ProfessionalSchedules
            .AsNoTracking()
            .AnyAsync(s =>
                (!ignoredScheduleId.HasValue || s.Id != ignoredScheduleId.Value) &&
                s.UserId == userId &&
                s.DayOfWeek == dayOfWeek &&
                s.StartTime < endTime &&
                s.EndTime > startTime,
            ct);
    }

    public async Task<bool> HasCoverageForAppointment(
        Guid userId,
        Guid locationId,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        CancellationToken ct = default)
    {
        return await _dbContext.ProfessionalSchedules
            .AnyAsync(s =>
                s.UserId == userId &&
                s.LocationId == locationId &&
                s.DayOfWeek == dayOfWeek &&
                s.StartTime <= startTime &&
                s.EndTime >= endTime,
            ct);
    }

    public async Task<List<ProfessionalSchedule>> GetSchedulesByDayAsync(
        Guid userId,
        Guid locationId,
        DayOfWeek dayOfWeek,
        CancellationToken ct = default)
    {
        return await _dbContext.ProfessionalSchedules
            .Where(s =>
                s.UserId == userId &&
                s.LocationId == locationId &&
                s.DayOfWeek == dayOfWeek)
            .OrderBy(s => s.StartTime) 
            .ToListAsync(ct);
    }

    private IQueryable<ProfessionalScheduleDetails> BuildDetailsQuery()
    {
        return _dbContext.ProfessionalSchedules
            .AsNoTracking()
            .Join(_dbContext.Users.AsNoTracking(),
                schedule => schedule.UserId,
                professional => professional.Id,
                (schedule, professional) => new { schedule, professional })
            .Join(_dbContext.Locations.AsNoTracking(),
                x => x.schedule.LocationId,
                location => location.Id,
                (x, location) => new ProfessionalScheduleDetails
                {
                    Schedule = x.schedule,
                    ProfessionalName = x.professional.Name,
                    LocationName = location.Name
                });
    }

}
