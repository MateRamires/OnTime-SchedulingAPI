using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Repositories.Reports;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;

public class ReportsRepository : IReportsReadOnlyRepository
{
    private readonly AppDbContext _dbContext;

    public ReportsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<AppointmentReportDetails>> GetAppointmentsStartedInPeriodAsync(
        DateTime startUtc,
        DateTime endUtc,
        Guid? locationId,
        Guid? professionalId,
        Guid? serviceId,
        IReadOnlyCollection<AppointmentStatus>? statuses,
        CancellationToken ct = default)
    {
        var query = BuildAppointmentDetailsQuery()
            .Where(appointment => appointment.StartTimeUtc >= startUtc && appointment.StartTimeUtc < endUtc);

        if (locationId.HasValue)
            query = query.Where(appointment => appointment.LocationId == locationId.Value);

        if (professionalId.HasValue)
            query = query.Where(appointment => appointment.ProfessionalId == professionalId.Value);

        if (serviceId.HasValue)
            query = query.Where(appointment => appointment.ServiceId == serviceId.Value);

        if (statuses is { Count: > 0 })
            query = query.Where(appointment => statuses.Contains(appointment.Status));

        return await query
            .OrderBy(appointment => appointment.StartTimeUtc)
            .ThenBy(appointment => appointment.AppointmentId)
            .ToListAsync(ct);
    }

    public async Task<List<AppointmentReportDetails>> GetAppointmentsOverlappingPeriodAsync(
        DateTime startUtc,
        DateTime endUtc,
        Guid? locationId,
        Guid? professionalId,
        CancellationToken ct = default)
    {
        var query = BuildAppointmentDetailsQuery()
            .Where(appointment => appointment.StartTimeUtc < endUtc && appointment.EndTimeUtc > startUtc);

        if (locationId.HasValue)
            query = query.Where(appointment => appointment.LocationId == locationId.Value);

        if (professionalId.HasValue)
            query = query.Where(appointment => appointment.ProfessionalId == professionalId.Value);

        return await query
            .OrderBy(appointment => appointment.StartTimeUtc)
            .ThenBy(appointment => appointment.AppointmentId)
            .ToListAsync(ct);
    }

    public async Task<List<ProfessionalOccupancyScheduleDetails>> GetProfessionalSchedulesForOccupancyAsync(
        Guid? locationId,
        Guid? professionalId,
        CancellationToken ct = default)
    {
        var query = _dbContext.ProfessionalSchedules
            .AsNoTracking()
            .Join(_dbContext.Users.AsNoTracking(),
                schedule => schedule.UserId,
                professional => professional.Id,
                (schedule, professional) => new { schedule, professional })
            .Join(_dbContext.Locations.AsNoTracking(),
                x => x.schedule.LocationId,
                location => location.Id,
                (x, location) => new ProfessionalOccupancyScheduleDetails
                {
                    ProfessionalId = x.schedule.UserId,
                    ProfessionalName = x.professional.Name,
                    LocationId = x.schedule.LocationId,
                    LocationName = location.Name,
                    LocationTimeZoneId = location.TimeZoneId,
                    DayOfWeek = x.schedule.DayOfWeek,
                    StartTime = x.schedule.StartTime,
                    EndTime = x.schedule.EndTime
                });

        if (locationId.HasValue)
            query = query.Where(schedule => schedule.LocationId == locationId.Value);

        if (professionalId.HasValue)
            query = query.Where(schedule => schedule.ProfessionalId == professionalId.Value);

        return await query
            .OrderBy(schedule => schedule.ProfessionalName)
            .ThenBy(schedule => schedule.LocationName)
            .ThenBy(schedule => schedule.DayOfWeek)
            .ThenBy(schedule => schedule.StartTime)
            .ToListAsync(ct);
    }

    public async Task<List<ProfessionalOccupancyScheduleBlockDetails>> GetScheduleBlocksForOccupancyAsync(
        DateTime startUtc,
        DateTime endUtc,
        Guid? locationId,
        Guid? professionalId,
        CancellationToken ct = default)
    {
        var query = _dbContext.ScheduleBlocks
            .AsNoTracking()
            .Where(block => block.StartTime < endUtc && block.EndTime > startUtc);

        if (locationId.HasValue)
            query = query.Where(block => !block.LocationId.HasValue || block.LocationId == locationId.Value);

        if (professionalId.HasValue)
            query = query.Where(block => !block.ProfessionalId.HasValue || block.ProfessionalId == professionalId.Value);

        return await query
            .Select(block => new ProfessionalOccupancyScheduleBlockDetails
            {
                ProfessionalId = block.ProfessionalId,
                LocationId = block.LocationId,
                StartTimeUtc = block.StartTime,
                EndTimeUtc = block.EndTime
            })
            .OrderBy(block => block.StartTimeUtc)
            .ToListAsync(ct);
    }

    private IQueryable<AppointmentReportDetails> BuildAppointmentDetailsQuery()
    {
        return _dbContext.Appointments
            .AsNoTracking()
            .Join(_dbContext.Users.AsNoTracking(),
                appointment => appointment.ProfessionalId,
                professional => professional.Id,
                (appointment, professional) => new { appointment, professional })
            .Join(_dbContext.Services.AsNoTracking(),
                x => x.appointment.ServiceId,
                service => service.Id,
                (x, service) => new { x.appointment, x.professional, service })
            .Join(_dbContext.Locations.AsNoTracking(),
                x => x.appointment.LocationId,
                location => location.Id,
                (x, location) => new AppointmentReportDetails
                {
                    AppointmentId = x.appointment.Id,
                    ProfessionalId = x.appointment.ProfessionalId,
                    ProfessionalName = x.professional.Name,
                    LocationId = x.appointment.LocationId,
                    LocationName = location.Name,
                    ServiceId = x.appointment.ServiceId,
                    ServiceName = x.service.Name,
                    Status = x.appointment.Status,
                    StartTimeUtc = x.appointment.StartTime,
                    EndTimeUtc = x.appointment.EndTime
                });
    }
}
