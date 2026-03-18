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

    public async Task<bool> HasOverlappingSchedule(
        Guid userId,
        DayOfWeek dayOfWeek,
        TimeSpan startTime,
        TimeSpan endTime,
        CancellationToken ct = default)
    {
        return await _dbContext.ProfessionalSchedules
            .AnyAsync(s =>
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

}
