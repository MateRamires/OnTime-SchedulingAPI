using OnTimeScheduling.Application.Repositories.Reports;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Communication.Enums;
using OnTimeScheduling.Communication.Requests.Reports;
using OnTimeScheduling.Communication.Responses.Reports;
using OnTimeScheduling.Exceptions.ExceptionBase;
using CommunicationAppointmentStatus = OnTimeScheduling.Communication.Enums.AppointmentStatus;
using DomainAppointmentStatus = OnTimeScheduling.Domain.Enums.AppointmentStatus;

namespace OnTimeScheduling.Application.UseCases.Reports;

public class GetAppointmentsVolumeReportUseCase : IGetAppointmentsVolumeReportUseCase
{
    private readonly IReportsReadOnlyRepository _reportsReadRepository;
    private readonly ITenantProvider _tenantProvider;

    public GetAppointmentsVolumeReportUseCase(
        IReportsReadOnlyRepository reportsReadRepository,
        ITenantProvider tenantProvider)
    {
        _reportsReadRepository = reportsReadRepository;
        _tenantProvider = tenantProvider;
    }

    public async Task<ResponseAppointmentsVolumeReportJson> ExecuteAsync(
        RequestAppointmentsVolumeReportJson request,
        CancellationToken ct = default)
    {
        _ = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var (startUtc, endUtc) = ValidateAndNormalizePeriod(request.StartTimeUtc, request.EndTimeUtc);

        if (!Enum.IsDefined(request.GroupBy))
            throw new ErrorOnValidationException(["GroupBy is invalid."]);

        var statuses = request.Status?
            .Distinct()
            .Select(status => (DomainAppointmentStatus)(int)status)
            .ToList();

        var appointments = await _reportsReadRepository.GetAppointmentsStartedInPeriodAsync(
            startUtc,
            endUtc,
            request.LocationId,
            request.ProfessionalId,
            request.ServiceId,
            statuses,
            ct);

        var items = appointments
            .GroupBy(appointment =>
            {
                var periodStart = GetPeriodStart(appointment.StartTimeUtc, request.GroupBy);

                return new
                {
                    PeriodStart = periodStart,
                    PeriodEnd = GetPeriodEnd(periodStart, request.GroupBy),
                    appointment.LocationId,
                    appointment.LocationName,
                    appointment.ProfessionalId,
                    appointment.ProfessionalName,
                    appointment.ServiceId,
                    appointment.ServiceName,
                    appointment.Status
                };
            })
            .Select(group => new ResponseAppointmentsVolumeReportItemJson
            {
                PeriodStartUtc = group.Key.PeriodStart,
                PeriodEndUtc = group.Key.PeriodEnd,
                LocationId = group.Key.LocationId,
                LocationName = group.Key.LocationName,
                ProfessionalId = group.Key.ProfessionalId,
                ProfessionalName = group.Key.ProfessionalName,
                ServiceId = group.Key.ServiceId,
                ServiceName = group.Key.ServiceName,
                Status = (CommunicationAppointmentStatus)(int)group.Key.Status,
                AppointmentsCount = group.Count(),
                TotalDurationInMinutes = group.Sum(appointment => GetDurationInMinutes(appointment.StartTimeUtc, appointment.EndTimeUtc))
            })
            .OrderBy(item => item.PeriodStartUtc)
            .ThenBy(item => item.LocationName)
            .ThenBy(item => item.ProfessionalName)
            .ThenBy(item => item.ServiceName)
            .ThenBy(item => item.Status)
            .ToList();

        return new ResponseAppointmentsVolumeReportJson
        {
            StartTimeUtc = startUtc,
            EndTimeUtc = endUtc,
            TotalAppointments = appointments.Count,
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

    private static DateTime GetPeriodStart(DateTime value, ReportPeriodGrouping groupBy)
    {
        var date = value.Date;

        return groupBy switch
        {
            ReportPeriodGrouping.DAY => DateTime.SpecifyKind(date, DateTimeKind.Utc),
            ReportPeriodGrouping.WEEK => DateTime.SpecifyKind(date.AddDays(-GetDaysSinceMonday(date.DayOfWeek)), DateTimeKind.Utc),
            ReportPeriodGrouping.MONTH => new DateTime(value.Year, value.Month, 1, 0, 0, 0, DateTimeKind.Utc),
            _ => throw new ErrorOnValidationException(["GroupBy is invalid."])
        };
    }

    private static DateTime GetPeriodEnd(DateTime periodStart, ReportPeriodGrouping groupBy)
    {
        return groupBy switch
        {
            ReportPeriodGrouping.DAY => periodStart.AddDays(1),
            ReportPeriodGrouping.WEEK => periodStart.AddDays(7),
            ReportPeriodGrouping.MONTH => periodStart.AddMonths(1),
            _ => throw new ErrorOnValidationException(["GroupBy is invalid."])
        };
    }

    private static int GetDaysSinceMonday(DayOfWeek dayOfWeek)
    {
        return dayOfWeek == DayOfWeek.Sunday ? 6 : (int)dayOfWeek - (int)DayOfWeek.Monday;
    }

    private static int GetDurationInMinutes(DateTime startUtc, DateTime endUtc)
    {
        return (int)Math.Round((endUtc - startUtc).TotalMinutes, MidpointRounding.AwayFromZero);
    }
}
