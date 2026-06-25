using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Domain.Entities.Appointments;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;

public class AppointmentRepository : IAppointmentWriteOnlyRepository, IAppointmentReadOnlyRepository
{
    private readonly AppDbContext _dbContext;

    public AppointmentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Appointment appointment, CancellationToken ct = default)
    {
        await _dbContext.Appointments.AddAsync(appointment, ct);
    }

    public void Update(Appointment appointment)
    {
        _dbContext.Appointments.Update(appointment);
    }

    public async Task<bool> HasOverlappingAppointment(
        Guid professionalId,
        DateTime newAppointmentStartTime,
        DateTime newAppointmentEndTime,
        CancellationToken ct = default,
        Guid? ignoredAppointmentId = null)
    {
        return await _dbContext.Appointments
            .AnyAsync(a =>
                a.ProfessionalId == professionalId &&
                a.Status != AppointmentStatus.Canceled &&
                (!ignoredAppointmentId.HasValue || a.Id != ignoredAppointmentId.Value) &&
                a.EndTime > newAppointmentStartTime && newAppointmentEndTime > a.StartTime,
            ct);
    }

    public async Task<List<Appointment>> GetAppointmentsByPeriod(
        Guid professionalId,
        DateTime startPeriod,
        DateTime endPeriod,
        CancellationToken ct = default)
    {
        return await _dbContext.Appointments
            .Where(a =>
                a.ProfessionalId == professionalId &&
                a.Status != AppointmentStatus.Canceled &&
                a.StartTime >= startPeriod &&
                a.StartTime < endPeriod)
            .OrderBy(a => a.StartTime)
            .ToListAsync(ct);
    }

    public async Task<List<Appointment>> GetAppointmentsByDateRangeAsync(
        Guid professionalId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken ct = default)
    {
        return await _dbContext.Appointments
            .AsNoTracking()
            .Where(a =>
                a.ProfessionalId == professionalId &&
                a.Status != AppointmentStatus.Canceled &&
                a.StartTime < endUtc &&
                a.EndTime > startUtc)
            .OrderBy(a => a.StartTime)
            .ToListAsync(ct);
    }

    public async Task<bool> HasOverlappingAppointmentForBlockAsync(
        Guid? professionalId,
        Guid? locationId,
        DateTime startTimeUtc,
        DateTime endTimeUtc,
        CancellationToken ct = default)
    {
        var nowUtc = DateTime.UtcNow;

        var query = _dbContext.Appointments
            .AsNoTracking()
            .Where(a =>
                a.Status != AppointmentStatus.Canceled &&
                a.EndTime > nowUtc &&
                a.StartTime < endTimeUtc &&
                a.EndTime > startTimeUtc);

        if (professionalId.HasValue)
            query = query.Where(a => a.ProfessionalId == professionalId.Value);

        if (locationId.HasValue)
            query = query.Where(a => a.LocationId == locationId.Value);

        return await query.AnyAsync(ct);
    }

    public async Task<bool> HasFutureScheduledAppointmentsAsync(
        Guid? professionalId = null,
        Guid? locationId = null,
        Guid? serviceId = null,
        Guid? clientId = null,
        CancellationToken ct = default)
    {
        var nowUtc = DateTime.UtcNow;

        var query = _dbContext.Appointments
            .AsNoTracking()
            .Where(a =>
                a.Status == AppointmentStatus.Scheduled &&
                a.EndTime > nowUtc);

        if (professionalId.HasValue)
            query = query.Where(a => a.ProfessionalId == professionalId.Value);

        if (locationId.HasValue)
            query = query.Where(a => a.LocationId == locationId.Value);

        if (serviceId.HasValue)
            query = query.Where(a => a.ServiceId == serviceId.Value);

        if (clientId.HasValue)
            query = query.Where(a => a.ClientId == clientId.Value);

        return await query.AnyAsync(ct);
    }

    public async Task<List<Appointment>> GetFutureScheduledAppointmentsForProfessionalLocationAsync(
        Guid professionalId,
        Guid locationId,
        CancellationToken ct = default)
    {
        var nowUtc = DateTime.UtcNow;

        return await _dbContext.Appointments
            .AsNoTracking()
            .Where(a =>
                a.ProfessionalId == professionalId &&
                a.LocationId == locationId &&
                a.Status == AppointmentStatus.Scheduled &&
                a.EndTime > nowUtc)
            .OrderBy(a => a.StartTime)
            .ToListAsync(ct);
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Appointments
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<List<AppointmentAgendaItem>> GetAgendaAsync(
        DateTime startUtc,
        DateTime endUtc,
        Guid? locationId,
        Guid? professionalId,
        AppointmentStatus? status,
        CancellationToken ct = default)
    {
        var query = _dbContext.Appointments.AsNoTracking()
            .Where(a => a.StartTime < endUtc && a.EndTime > startUtc);

        if (locationId.HasValue)
            query = query.Where(a => a.LocationId == locationId.Value);

        if (professionalId.HasValue)
            query = query.Where(a => a.ProfessionalId == professionalId.Value);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        return await query
            .Join(_dbContext.Clients, a => a.ClientId, c => c.Id, (a, c) => new { a, c })
            .Join(_dbContext.Users, x => x.a.ProfessionalId, u => u.Id, (x, u) => new { x.a, x.c, u })
            .Join(_dbContext.Services, x => x.a.ServiceId, s => s.Id, (x, s) => new { x.a, x.c, x.u, s })
            .Join(_dbContext.Locations, x => x.a.LocationId, l => l.Id, (x, l) => new AppointmentAgendaItem
            {
                AppointmentId = x.a.Id,
                ClientId = x.c.Id,
                ClientName = x.c.Name,
                ProfessionalId = x.u.Id,
                ProfessionalName = x.u.Name,
                ServiceId = x.s.Id,
                ServiceName = x.s.Name,
                LocationId = l.Id,
                LocationName = l.Name,
                Status = x.a.Status,
                StartTimeUtc = x.a.StartTime,
                EndTimeUtc = x.a.EndTime
            })
            .OrderBy(x => x.StartTimeUtc)
            .ToListAsync(ct);
    }

    public async Task<AppointmentDetails?> GetAppointmentDetailsByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await BuildAppointmentDetailsQuery()
            .FirstOrDefaultAsync(a => a.AppointmentId == id, ct);
    }

    public async Task<(List<AppointmentDetails> Items, int TotalItems)> GetAppointmentsAsync(
        int skip,
        int take,
        Guid? locationId,
        Guid? professionalId,
        Guid? clientId,
        Guid? serviceId,
        IReadOnlyCollection<AppointmentStatus>? statuses,
        DateTime? startTimeUtc,
        DateTime? endTimeUtc,
        CancellationToken ct = default)
    {
        var query = BuildAppointmentDetailsQuery();

        if (locationId.HasValue)
            query = query.Where(a => a.LocationId == locationId.Value);

        if (professionalId.HasValue)
            query = query.Where(a => a.ProfessionalId == professionalId.Value);

        if (clientId.HasValue)
            query = query.Where(a => a.ClientId == clientId.Value);

        if (serviceId.HasValue)
            query = query.Where(a => a.ServiceId == serviceId.Value);

        if (statuses is { Count: > 0 })
            query = query.Where(a => statuses.Contains(a.Status));

        if (startTimeUtc.HasValue)
            query = query.Where(a => a.EndTimeUtc > startTimeUtc.Value);

        if (endTimeUtc.HasValue)
            query = query.Where(a => a.StartTimeUtc < endTimeUtc.Value);

        var totalItems = await query.CountAsync(ct);

        var items = await query
            .OrderBy(a => a.StartTimeUtc)
            .ThenBy(a => a.ProfessionalName)
            .ThenBy(a => a.ClientName)
            .ThenBy(a => a.AppointmentId)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

        return (items, totalItems);
    }

    private IQueryable<AppointmentDetails> BuildAppointmentDetailsQuery()
    {
        return _dbContext.Appointments
            .AsNoTracking()
            .Join(_dbContext.Clients, a => a.ClientId, c => c.Id, (a, c) => new { a, c })
            .Join(_dbContext.Users, x => x.a.ProfessionalId, u => u.Id, (x, u) => new { x.a, x.c, u })
            .Join(_dbContext.Services, x => x.a.ServiceId, s => s.Id, (x, s) => new { x.a, x.c, x.u, s })
            .Join(_dbContext.Locations, x => x.a.LocationId, l => l.Id, (x, l) => new AppointmentDetails
            {
                AppointmentId = x.a.Id,
                ClientId = x.c.Id,
                ClientName = x.c.Name,
                ClientPhone = x.c.Phone,
                ClientEmail = x.c.Email,
                ProfessionalId = x.u.Id,
                ProfessionalName = x.u.Name,
                LocationId = l.Id,
                LocationName = l.Name,
                ServiceId = x.s.Id,
                ServiceName = x.s.Name,
                ServiceDurationInMinutes = x.s.DurationInMinutes,
                ServicePrice = x.s.Price,
                Status = x.a.Status,
                StartTimeUtc = x.a.StartTime,
                EndTimeUtc = x.a.EndTime,
                CreatedAtUtc = x.a.CreatedAt,
                UpdatedAtUtc = x.a.UpdatedAt
            });
    }
}
