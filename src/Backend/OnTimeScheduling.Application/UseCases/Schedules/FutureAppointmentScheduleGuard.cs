using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Domain.Entities.Appointments;
using OnTimeScheduling.Domain.Entities.Schedules;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Schedules;

public sealed class FutureAppointmentScheduleGuard
{
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly ILocationReadOnlyRepository _locationReadRepository;

    public FutureAppointmentScheduleGuard(
        IAppointmentReadOnlyRepository appointmentReadRepository,
        ILocationReadOnlyRepository locationReadRepository)
    {
        _appointmentReadRepository = appointmentReadRepository;
        _locationReadRepository = locationReadRepository;
    }

    public async Task EnsureCanDeleteAsync(ProfessionalSchedule schedule, CancellationToken ct)
    {
        var affectedAppointment = await FindFutureAppointmentCoveredByCurrentScheduleAsync(schedule, ct);

        if (affectedAppointment is not null)
            throw new ConflictException("Cannot delete a professional schedule with future scheduled appointments. Cancel or reschedule those appointments first.");
    }

    public async Task EnsureCanUpdateAsync(
        ProfessionalSchedule currentSchedule,
        RequestUpdateScheduleJson request,
        CancellationToken ct)
    {
        var affectedAppointments = await GetFutureAppointmentsCoveredByCurrentScheduleAsync(currentSchedule, ct);
        if (affectedAppointments.Count == 0)
            return;

        var timeZone = await GetTimeZoneInfoAsync(currentSchedule.LocationId, ct);

        var breaksExistingAppointments = affectedAppointments.Any(appointment =>
            !IsCoveredByRequest(appointment, request, timeZone));

        if (breaksExistingAppointments)
            throw new ConflictException("Cannot update a professional schedule in a way that leaves future scheduled appointments outside the professional's agenda. Cancel or reschedule those appointments first.");
    }

    private async Task<Appointment?> FindFutureAppointmentCoveredByCurrentScheduleAsync(
        ProfessionalSchedule schedule,
        CancellationToken ct)
    {
        var appointments = await GetFutureAppointmentsCoveredByCurrentScheduleAsync(schedule, ct);
        return appointments.FirstOrDefault();
    }

    private async Task<List<Appointment>> GetFutureAppointmentsCoveredByCurrentScheduleAsync(
        ProfessionalSchedule schedule,
        CancellationToken ct)
    {
        var timeZone = await GetTimeZoneInfoAsync(schedule.LocationId, ct);

        var appointments = await _appointmentReadRepository.GetFutureScheduledAppointmentsForProfessionalLocationAsync(
            schedule.UserId,
            schedule.LocationId,
            ct);

        return appointments
            .Where(appointment => IsCoveredBySchedule(appointment, schedule, timeZone))
            .ToList();
    }

    private async Task<TimeZoneInfo> GetTimeZoneInfoAsync(Guid locationId, CancellationToken ct)
    {
        var timeZoneId = await _locationReadRepository.GetLocationTimeZoneIdById(locationId, ct)
            ?? throw new NotFoundException("Location not found.");

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException || ex is InvalidTimeZoneException)
        {
            throw new ErrorOnValidationException(["Invalid location time zone configuration."]);
        }
    }

    private static bool IsCoveredBySchedule(
        Appointment appointment,
        ProfessionalSchedule schedule,
        TimeZoneInfo timeZone)
    {
        var (dayOfWeek, startTime, endTime, spansMultipleDays) = GetLocalAppointmentTime(appointment, timeZone);

        return !spansMultipleDays &&
            dayOfWeek == schedule.DayOfWeek &&
            schedule.StartTime <= startTime &&
            schedule.EndTime >= endTime;
    }

    private static bool IsCoveredByRequest(
        Appointment appointment,
        RequestUpdateScheduleJson request,
        TimeZoneInfo timeZone)
    {
        if (appointment.ProfessionalId != request.UserId || appointment.LocationId != request.LocationId)
            return false;

        var (dayOfWeek, startTime, endTime, spansMultipleDays) = GetLocalAppointmentTime(appointment, timeZone);

        return !spansMultipleDays &&
            dayOfWeek == request.DayOfWeek &&
            request.StartTime <= startTime &&
            request.EndTime >= endTime;
    }

    private static (DayOfWeek DayOfWeek, TimeSpan StartTime, TimeSpan EndTime, bool SpansMultipleDays)
        GetLocalAppointmentTime(Appointment appointment, TimeZoneInfo timeZone)
    {
        var startUtc = DateTime.SpecifyKind(appointment.StartTime, DateTimeKind.Utc);
        var endUtc = DateTime.SpecifyKind(appointment.EndTime, DateTimeKind.Utc);

        var localStart = TimeZoneInfo.ConvertTimeFromUtc(startUtc, timeZone);
        var localEnd = TimeZoneInfo.ConvertTimeFromUtc(endUtc, timeZone);

        return (
            localStart.DayOfWeek,
            localStart.TimeOfDay,
            localEnd.TimeOfDay,
            localStart.Date != localEnd.Date);
    }
}
