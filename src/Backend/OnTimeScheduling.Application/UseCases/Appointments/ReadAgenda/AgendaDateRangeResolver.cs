using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Domain.Entities.Locations;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Appointments.ReadAgenda;

internal static class AgendaDateRangeResolver
{
    public static async Task<(DateTime StartUtc, DateTime EndUtc)> ResolveUtcRangeAsync(
        DateOnly localDate,
        int days,
        Guid? locationId,
        ILocationReadOnlyRepository locationReadRepository,
        CancellationToken ct)
    {
        if (days <= 0)
            throw new ErrorOnValidationException(["Agenda range must be greater than zero days."]);

        var timeZoneId = Location.DefaultTimeZoneId;

        if (locationId.HasValue)
        {
            var locationTimeZoneId = await locationReadRepository.GetActiveLocationTimeZoneIdById(locationId.Value, ct);

            if (locationTimeZoneId is null)
                throw new ErrorOnValidationException(["Location not found in this tenant."]);

            timeZoneId = locationTimeZoneId;
        }

        var timeZone = GetTimeZoneInfo(timeZoneId);
        var localStart = localDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
        var localEnd = localDate.AddDays(days).ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);

        return (
            ConvertLocalBoundaryToUtc(localStart, timeZone, isStartBoundary: true),
            ConvertLocalBoundaryToUtc(localEnd, timeZone, isStartBoundary: false));
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
            throw new ErrorOnValidationException(["The selected agenda date contains an invalid local time for the location timezone."]);

        if (timeZone.IsAmbiguousTime(localDateTime))
        {
            var utcCandidates = timeZone.GetAmbiguousTimeOffsets(localDateTime)
                .Select(offset => DateTime.SpecifyKind(localDateTime - offset, DateTimeKind.Utc))
                .ToList();

            return isStartBoundary ? utcCandidates.Min() : utcCandidates.Max();
        }

        return TimeZoneInfo.ConvertTimeToUtc(localDateTime, timeZone);
    }
}
