using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Validators.Appointments;
using OnTimeScheduling.Communication.Requests.Appointments;
using OnTimeScheduling.Communication.Responses.Appointments;
using OnTimeScheduling.Exceptions.ExceptionBase;
using CommunicationAppointmentStatus = OnTimeScheduling.Communication.Enums.AppointmentStatus;
using DomainAppointmentStatus = OnTimeScheduling.Domain.Enums.AppointmentStatus;

namespace OnTimeScheduling.Application.UseCases.Appointments.ReadAgenda;

public class GetDailyAgendaUseCase : IGetDailyAgendaUseCase
{
    private readonly IAppointmentReadOnlyRepository _repo;
    private readonly ILocationReadOnlyRepository _locationReadRepository;

    public GetDailyAgendaUseCase(
        IAppointmentReadOnlyRepository repo,
        ILocationReadOnlyRepository locationReadRepository)
    {
        _repo = repo;
        _locationReadRepository = locationReadRepository;
    }

    public async Task<ResponseAgendaJson> ExecuteAsync(RequestGetDailyAgendaJson request, CancellationToken ct = default)
    {
        ValidateRequest(request);

        var (startUtc, endUtc) = await AgendaDateRangeResolver.ResolveUtcRangeAsync(
            request.Date,
            days: 1,
            request.LocationId,
            _locationReadRepository,
            ct);

        var status = request.Status.HasValue ? (DomainAppointmentStatus?)(int)request.Status.Value : null;
        var items = await _repo.GetAgendaAsync(startUtc, endUtc, request.LocationId, request.ProfessionalId, status, ct);

        return new ResponseAgendaJson
        {
            RangeStartUtc = startUtc,
            RangeEndUtc = endUtc,
            Items = items.Select(MapAgendaItem).ToList()
        };
    }

    private static ResponseAppointmentAgendaItemJson MapAgendaItem(AppointmentAgendaItem item)
    {
        return new ResponseAppointmentAgendaItemJson
        {
            AppointmentId = item.AppointmentId,
            ClientId = item.ClientId,
            ClientName = item.ClientName,
            ProfessionalId = item.ProfessionalId,
            ProfessionalName = item.ProfessionalName,
            LocationId = item.LocationId,
            LocationName = item.LocationName,
            ServiceId = item.ServiceId,
            ServiceName = item.ServiceName,
            Status = (CommunicationAppointmentStatus)(int)item.Status,
            StartTimeUtc = item.StartTimeUtc,
            EndTimeUtc = item.EndTimeUtc
        };
    }

    private static void ValidateRequest(RequestGetDailyAgendaJson request)
    {
        var validator = new GetDailyAgendaValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
    }


}
