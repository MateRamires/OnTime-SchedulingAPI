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

    public async Task<Appointment?> GetAppointmentByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbContext.Appointments
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }
}
