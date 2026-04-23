using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Repositories.Schedules;
using OnTimeScheduling.Application.Repositories.Services;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Validators.Appointments;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public class GetAvailableTimeSlotsUseCase : IGetAvailableTimeSlotsUseCase
{
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly IProfessionalScheduleReadOnlyRepository _scheduleReadRepository;
    private readonly IProfessionalServiceReadOnlyRepository _professionalServiceReadRepository;
    private readonly IServiceReadOnlyRepository _serviceReadRepository;
    private readonly ILocationReadOnlyRepository _locationReadOnlyRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITenantProvider _tenantProvider;


    public GetAvailableTimeSlotsUseCase(
        IAppointmentReadOnlyRepository appointmentReadRepository,
        IProfessionalScheduleReadOnlyRepository scheduleReadRepository,
        IServiceReadOnlyRepository serviceReadRepository,
        IProfessionalServiceReadOnlyRepository professionalServiceReadRepository,
        ILocationReadOnlyRepository locationReadOnlyRepository,
        IUserRepository userRepository,
        ITenantProvider tenantProvider)
    {
        _appointmentReadRepository = appointmentReadRepository;
        _scheduleReadRepository = scheduleReadRepository;
        _serviceReadRepository = serviceReadRepository;
        _professionalServiceReadRepository = professionalServiceReadRepository;
        _locationReadOnlyRepository = locationReadOnlyRepository;
        _userRepository = userRepository;
        _tenantProvider = tenantProvider;

    }

    public async Task<ResponseAvailableTimeSlotsJson> ExecuteAsync(RequestGetAvailableTimeSlotsJson request, CancellationToken ct = default)
    {
        ValidateRequest(request);

        var errors = new List<string>();

        if (!_tenantProvider.CompanyId.HasValue)
            errors.Add("The authenticated user does not have a valid tenant context.");

        var service = await _serviceReadRepository.GetByIdAsync(request.ServiceId, ct);
        if (service is null)
            errors.Add("Service not found.");

        var locationTimeZoneId = await _locationReadOnlyRepository.GetActiveLocationTimeZoneIdById(request.LocationId, ct);
        if (locationTimeZoneId is null)
            errors.Add("Location not found in this tenant.");

        if (_tenantProvider.CompanyId.HasValue)
        {
            var professional = await _userRepository.GetByIdAndCompany(request.ProfessionalId, _tenantProvider.CompanyId.Value, ct);
            if (professional is null)
                errors.Add("Professional not found in this tenant.");
            else if (professional.Role != UserRole.PROVIDER)
                errors.Add("Only provider users can receive appointments.");
        }

        var doesProfessionalPerformService = await _professionalServiceReadRepository.Exists(request.ProfessionalId, request.ServiceId, ct);
        if (!doesProfessionalPerformService)
            errors.Add("This professional does not provide the selected service.");

        if (errors.Count != 0)
            throw new ErrorOnValidationException(errors);

        var timeZone = GetTimeZoneInfo(locationTimeZoneId!);
        var localTargetDate = request.TargetDate;
        var currentLocalDate = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone));

        if (localTargetDate < currentLocalDate)
            throw new ErrorOnValidationException(["Cannot search for available slots in the past."]);

        var localStartOfDay = localTargetDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
        var localEndOfDay = localTargetDate.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
        var dayOfWeek = localStartOfDay.DayOfWeek;

        var schedules = await _scheduleReadRepository.GetSchedulesByDayAsync(
            request.ProfessionalId, request.LocationId, dayOfWeek, ct);

        if (schedules.Count == 0)
            return new ResponseAvailableTimeSlotsJson { AvailableSlotsUtc = [] };

        var utcStartOfDay = ConvertLocalBoundaryToUtc(localStartOfDay, timeZone, isStartBoundary: true);
        var utcEndOfDay = ConvertLocalBoundaryToUtc(localEndOfDay, timeZone, isStartBoundary: false);

        var appointments = await _appointmentReadRepository.GetAppointmentsByDateRangeAsync(
            request.ProfessionalId, utcStartOfDay, utcEndOfDay, ct);

        var availableSlotsUtc = new HashSet<DateTime>();
        var serviceDuration = TimeSpan.FromMinutes(service!.DurationInMinutes);
        var nowUtc = DateTime.UtcNow;

        foreach (var schedule in schedules)
        {
            var currentSlotLocalTime = schedule.StartTime;
            var scheduleEndLocalTime = schedule.EndTime;

            while (currentSlotLocalTime.Add(serviceDuration) <= scheduleEndLocalTime)
            {
                var slotLocalStartDateTime = localTargetDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified)
                    .Add(currentSlotLocalTime);
                var slotLocalEndDateTime = slotLocalStartDateTime.Add(serviceDuration);

                var slotUtcCandidates = GetUtcSlotCandidates(slotLocalStartDateTime, slotLocalEndDateTime, timeZone, serviceDuration);

                foreach (var (slotUtcStart, slotUtcEnd) in slotUtcCandidates)
                {
                    if (slotUtcStart <= nowUtc)
                        continue;

                    var hasOverlap = appointments.Any(a =>
                        slotUtcStart < a.EndTime &&
                        slotUtcEnd > a.StartTime);

                    if (!hasOverlap)
                        availableSlotsUtc.Add(slotUtcStart);
                    
                }

                currentSlotLocalTime = currentSlotLocalTime.Add(serviceDuration);
            }
        }

        return new ResponseAvailableTimeSlotsJson
        {
            AvailableSlotsUtc = availableSlotsUtc.OrderBy(slot => slot).ToList()
        };
    }

    private void ValidateRequest(RequestGetAvailableTimeSlotsJson request)
    {
        var validator = new GetAvailableTimeSlotsValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errors);
        }
    }

    private TimeZoneInfo GetTimeZoneInfo(string timeZoneId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException || ex is InvalidTimeZoneException)
        {
            throw new ErrorOnValidationException(["Invalid location time zone configuration."]);
        }
    }

    private static DateTime ConvertLocalBoundaryToUtc(DateTime localDateTime, TimeZoneInfo timeZone, bool isStartBoundary)
    {
        if (timeZone.IsInvalidTime(localDateTime))
            throw new ErrorOnValidationException(["The selected date contains an invalid local time for the location timezone."]);


        if (timeZone.IsAmbiguousTime(localDateTime))
        {
            var offsets = timeZone.GetAmbiguousTimeOffsets(localDateTime);
            var utcCandidates = offsets
                .Select(offset => DateTime.SpecifyKind(localDateTime - offset, DateTimeKind.Utc))
                .ToList();

            return isStartBoundary ? utcCandidates.Min() : utcCandidates.Max();
        }


        return TimeZoneInfo.ConvertTimeToUtc(localDateTime, timeZone);
    }

    private static List<DateTime> GetUtcCandidates(DateTime localDateTime, TimeZoneInfo timeZone)
    {
        if (timeZone.IsInvalidTime(localDateTime))
            return [];

        if (!timeZone.IsAmbiguousTime(localDateTime))
            return [TimeZoneInfo.ConvertTimeToUtc(localDateTime, timeZone)];

        var offsets = timeZone.GetAmbiguousTimeOffsets(localDateTime);
        return offsets
            .Select(offset => DateTime.SpecifyKind(localDateTime - offset, DateTimeKind.Utc))
            .Distinct()
            .ToList();

    }

    private static List<(DateTime StartUtc, DateTime EndUtc)> GetUtcSlotCandidates(
        DateTime localStartDateTime,
        DateTime localEndDateTime,
        TimeZoneInfo timeZone,
        TimeSpan serviceDuration)
    {
        var startCandidates = GetUtcCandidates(localStartDateTime, timeZone);
        var endCandidates = GetUtcCandidates(localEndDateTime, timeZone);

        if (startCandidates.Count == 0 || endCandidates.Count == 0)
            return [];

        return startCandidates
            .SelectMany(startUtc => endCandidates
                .Where(endUtc => endUtc > startUtc && endUtc - startUtc == serviceDuration)
                .Select(endUtc => (StartUtc: startUtc, EndUtc: endUtc)))
            .Distinct()
            .ToList();
    }


}
