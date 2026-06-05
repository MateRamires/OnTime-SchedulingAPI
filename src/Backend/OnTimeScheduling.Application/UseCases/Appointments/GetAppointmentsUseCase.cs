using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public class GetAppointmentsUseCase : IGetAppointmentsUseCase
{
    private readonly IAppointmentReadOnlyRepository _appointmentReadRepository;
    private readonly ITenantProvider _tenantProvider;

    public GetAppointmentsUseCase(IAppointmentReadOnlyRepository appointmentReadRepository, ITenantProvider tenantProvider)
    {
        _appointmentReadRepository = appointmentReadRepository;
        _tenantProvider = tenantProvider;
    }

    public async Task<ResponsePagedResultJson<ResponseAppointmentSummaryJson>> ExecuteAsync(RequestGetAppointmentsJson request, CancellationToken ct = default)
    {
        _ = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var startTimeUtc = NormalizeUtc(request.StartTimeUtc);
        var endTimeUtc = NormalizeUtc(request.EndTimeUtc);

        if (startTimeUtc.HasValue && endTimeUtc.HasValue && startTimeUtc.Value >= endTimeUtc.Value)
            throw new ErrorOnValidationException(["StartTimeUtc must be before EndTimeUtc."]);

        var statuses = request.Status?
            .Distinct()
            .Select(status => (DomainAppointmentStatus)(int)status)
            .ToList();

        var (appointments, totalItems) = await _appointmentReadRepository.GetAppointmentsAsync(
            request.Skip,
            request.Size,
            request.LocationId,
            request.ProfessionalId,
            request.ClientId,
            request.ServiceId,
            statuses,
            startTimeUtc,
            endTimeUtc,
            ct);

        var items = appointments.Select(AppointmentResponseMapper.MapSummary).ToList();

        return new ResponsePagedResultJson<ResponseAppointmentSummaryJson>
        {
            Page = request.Page,
            Size = request.Size,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.Size),
            Items = items
        };
    }

    private static DateTime? NormalizeUtc(DateTime? value)
    {
        if (!value.HasValue)
            return null;

        return value.Value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
            : value.Value.ToUniversalTime();
    }

}
