using OnTimeScheduling.Application.Repositories.Reports;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Communication.Requests.Reports;
using OnTimeScheduling.Communication.Responses.Reports;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Reports;

public class GetProfessionalOccupancyReportUseCase : IGetProfessionalOccupancyReportUseCase
{
    private readonly IReportsReadOnlyRepository _reportsReadRepository;
    private readonly ITenantProvider _tenantProvider;

    public GetProfessionalOccupancyReportUseCase(
        IReportsReadOnlyRepository reportsReadRepository,
        ITenantProvider tenantProvider)
    {
        _reportsReadRepository = reportsReadRepository;
        _tenantProvider = tenantProvider;
    }

    public async Task<ResponseProfessionalOccupancyReportJson> ExecuteAsync(
        RequestProfessionalOccupancyReportJson request,
        CancellationToken ct = default)
    {
        _ = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var (startUtc, endUtc) = ValidateAndNormalizePeriod(request.StartTimeUtc, request.EndTimeUtc);

        var schedules = await _reportsReadRepository.GetProfessionalSchedulesForOccupancyAsync(
            request.LocationId,
            request.ProfessionalId,
            ct);

        var appointments = await _reportsReadRepository.GetAppointmentsOverlappingPeriodAsync(
            startUtc,
            endUtc,
            request.LocationId,
            request.ProfessionalId,
            ct);

        var scheduleBlocks = await _reportsReadRepository.GetScheduleBlocksForOccupancyAsync(
            startUtc,
            endUtc,
            request.LocationId,
            request.ProfessionalId,
            ct);

        var rows = schedules
            .Select(schedule => new OccupancyRowKey(
                schedule.ProfessionalId,
                schedule.ProfessionalName,
                schedule.LocationId,
                schedule.LocationName))
            .Concat(appointments.Select(appointment => new OccupancyRowKey(
                appointment.ProfessionalId,
                appointment.ProfessionalName,
                appointment.LocationId,
                appointment.LocationName)))
            .GroupBy(key => new { key.ProfessionalId, key.LocationId })
            .Select(group => group.First())
            .OrderBy(key => key.ProfessionalName)
            .ThenBy(key => key.LocationName)
            .ToList();

        var items = rows.Select(row =>
        {
            var rowSchedules = schedules
                .Where(schedule => schedule.ProfessionalId == row.ProfessionalId && schedule.LocationId == row.LocationId)
                .ToList();

            var rowScheduleBlocks = scheduleBlocks
                .Where(block => MatchesRow(block, row.ProfessionalId, row.LocationId))
                .ToList();

            var rowAppointments = appointments
                .Where(appointment =>
                    appointment.ProfessionalId == row.ProfessionalId &&
                    appointment.LocationId == row.LocationId &&
                    appointment.Status != AppointmentStatus.Canceled)
                .ToList();

            var scheduledCapacity = CalculateScheduledCapacityInMinutes(rowSchedules, startUtc, endUtc);
            var blockedCapacity = CalculateBlockedCapacityInMinutes(rowSchedules, rowScheduleBlocks, startUtc, endUtc);
            var availableCapacity = Math.Max(scheduledCapacity - blockedCapacity, 0);
            var occupied = rowAppointments.Sum(appointment => CalculateOverlapInMinutes(
                appointment.StartTimeUtc,
                appointment.EndTimeUtc,
                startUtc,
                endUtc));

            return new ResponseProfessionalOccupancyReportItemJson
            {
                ProfessionalId = row.ProfessionalId,
                ProfessionalName = row.ProfessionalName,
                LocationId = row.LocationId,
                LocationName = row.LocationName,
                ScheduledCapacityInMinutes = scheduledCapacity,
                BlockedCapacityInMinutes = blockedCapacity,
                AvailableCapacityInMinutes = availableCapacity,
                OccupiedInMinutes = occupied,
                AppointmentsCount = rowAppointments.Count,
                OccupancyPercentage = availableCapacity == 0
                    ? 0
                    : Math.Round(occupied * 100m / availableCapacity, 2)
            };
        }).ToList();

        return new ResponseProfessionalOccupancyReportJson
        {
            StartTimeUtc = startUtc,
            EndTimeUtc = endUtc,
            Items = items
        };
    }

    private static (DateTime StartUtc, DateTime EndUtc) ValidateAndNormalizePeriod(DateTime? start, DateTime? end)
    {
        var errors = new List<string>();

        if (!start.HasValue)
            errors.Add("StartTimeUtc is required.");

        if (!end.HasValue)
            errors.Add("EndTimeUtc is required.");

        if (errors.Count != 0)
            throw new ErrorOnValidationException(errors);

        var startUtc = NormalizeUtc(start!.Value);
        var endUtc = NormalizeUtc(end!.Value);

        if (startUtc >= endUtc)
            throw new ErrorOnValidationException(["StartTimeUtc must be before EndTimeUtc."]);

        return (startUtc, endUtc);
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        return value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
            : value.ToUniversalTime();
    }

    private static bool MatchesRow(
        ProfessionalOccupancyScheduleBlockDetails block,
        Guid professionalId,
        Guid locationId)
    {
        var matchesProfessional = !block.ProfessionalId.HasValue || block.ProfessionalId == professionalId;
        var matchesLocation = !block.LocationId.HasValue || block.LocationId == locationId;

        return matchesProfessional && matchesLocation;
    }

    private static int CalculateScheduledCapacityInMinutes(
        List<ProfessionalOccupancyScheduleDetails> schedules,
        DateTime startUtc,
        DateTime endUtc)
    {
        return BuildScheduleIntervals(schedules, startUtc, endUtc)
            .Sum(interval => CalculateOverlapInMinutes(interval.StartUtc, interval.EndUtc, startUtc, endUtc));
    }

    private static int CalculateBlockedCapacityInMinutes(
        List<ProfessionalOccupancyScheduleDetails> schedules,
        List<ProfessionalOccupancyScheduleBlockDetails> blocks,
        DateTime startUtc,
        DateTime endUtc)
    {
        var blockedIntervals = new List<TimeInterval>();

        foreach (var scheduleInterval in BuildScheduleIntervals(schedules, startUtc, endUtc))
        {
            var overlaps = blocks
                .Where(block => block.StartTimeUtc < scheduleInterval.EndUtc && block.EndTimeUtc > scheduleInterval.StartUtc)
                .Select(block => new TimeInterval(
                    Max(scheduleInterval.StartUtc, block.StartTimeUtc, startUtc),
                    Min(scheduleInterval.EndUtc, block.EndTimeUtc, endUtc)))
                .Where(interval => interval.StartUtc < interval.EndUtc)
                .ToList();

            blockedIntervals.AddRange(MergeIntervals(overlaps));
        }

        return blockedIntervals.Sum(interval => GetDurationInMinutes(interval.StartUtc, interval.EndUtc));
    }

    private static List<TimeInterval> BuildScheduleIntervals(
        List<ProfessionalOccupancyScheduleDetails> schedules,
        DateTime startUtc,
        DateTime endUtc)
    {
        var intervals = new List<TimeInterval>();

        foreach (var schedule in schedules)
        {
            var timeZone = GetTimeZoneInfo(schedule.LocationTimeZoneId);
            var localStartDate = TimeZoneInfo.ConvertTimeFromUtc(startUtc, timeZone).Date;
            var localEndDate = TimeZoneInfo.ConvertTimeFromUtc(endUtc, timeZone).Date;

            for (var localDate = localStartDate; localDate <= localEndDate; localDate = localDate.AddDays(1))
            {
                if (schedule.DayOfWeek != localDate.DayOfWeek)
                    continue;

                var scheduleStartLocal = DateTime.SpecifyKind(localDate.Add(schedule.StartTime), DateTimeKind.Unspecified);
                var scheduleEndLocal = DateTime.SpecifyKind(localDate.Add(schedule.EndTime), DateTimeKind.Unspecified);
                var scheduleStart = ConvertLocalBoundaryToUtc(scheduleStartLocal, timeZone, isStartBoundary: true);
                var scheduleEnd = ConvertLocalBoundaryToUtc(scheduleEndLocal, timeZone, isStartBoundary: false);

                if (scheduleStart < endUtc && scheduleEnd > startUtc)
                    intervals.Add(new TimeInterval(scheduleStart, scheduleEnd));
            }
        }

        return intervals;
    }

    private static TimeZoneInfo GetTimeZoneInfo(string timeZoneId)
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
            throw new ErrorOnValidationException(["The selected period contains an invalid local time for the location timezone."]);

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

    private static List<TimeInterval> MergeIntervals(List<TimeInterval> intervals)
    {
        if (intervals.Count == 0)
            return [];

        var orderedIntervals = intervals
            .OrderBy(interval => interval.StartUtc)
            .ThenBy(interval => interval.EndUtc)
            .ToList();

        var merged = new List<TimeInterval> { orderedIntervals[0] };

        foreach (var current in orderedIntervals.Skip(1))
        {
            var previous = merged[^1];

            if (current.StartUtc <= previous.EndUtc)
            {
                merged[^1] = new TimeInterval(previous.StartUtc, Max(previous.EndUtc, current.EndUtc));
                continue;
            }

            merged.Add(current);
        }

        return merged;
    }

    private static int CalculateOverlapInMinutes(DateTime startUtc, DateTime endUtc, DateTime rangeStartUtc, DateTime rangeEndUtc)
    {
        var start = Max(startUtc, rangeStartUtc);
        var end = Min(endUtc, rangeEndUtc);

        if (start >= end)
            return 0;

        return GetDurationInMinutes(start, end);
    }

    private static int GetDurationInMinutes(DateTime startUtc, DateTime endUtc)
    {
        return (int)Math.Round((endUtc - startUtc).TotalMinutes, MidpointRounding.AwayFromZero);
    }

    private static DateTime Max(params DateTime[] values) => values.Max();

    private static DateTime Min(params DateTime[] values) => values.Min();

    private sealed record OccupancyRowKey(
        Guid ProfessionalId,
        string ProfessionalName,
        Guid LocationId,
        string LocationName);

    private sealed record TimeInterval(DateTime StartUtc, DateTime EndUtc);
}
