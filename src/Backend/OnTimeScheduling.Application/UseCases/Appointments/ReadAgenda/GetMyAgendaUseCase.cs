using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Repositories.Locations;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Application.Validators.Appointments;
using OnTimeScheduling.Communication.Requests.Appointments;
using OnTimeScheduling.Communication.Responses.Appointments;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;
using CommunicationAppointmentStatus = OnTimeScheduling.Communication.Enums.AppointmentStatus;
using DomainAgendaWindow = OnTimeScheduling.Domain.Enums.AgendaWindow;
using DomainAppointmentStatus = OnTimeScheduling.Domain.Enums.AppointmentStatus;

namespace OnTimeScheduling.Application.UseCases.Appointments.ReadAgenda;

public class GetMyAgendaUseCase : IGetMyAgendaUseCase
{
    private readonly IAppointmentReadOnlyRepository _repo;
    private readonly ILoggedUser _loggedUser;
    private readonly ILocationReadOnlyRepository _locationReadRepository;

    public GetMyAgendaUseCase(
        IAppointmentReadOnlyRepository repo,
        ILoggedUser loggedUser,
        ILocationReadOnlyRepository locationReadRepository)
    {
        _repo = repo;
        _loggedUser = loggedUser;
        _locationReadRepository = locationReadRepository;
    }

    public async Task<ResponseAgendaJson> ExecuteAsync(RequestGetMyAgendaJson request, CancellationToken ct = default)
    {
        ValidateRequest(request);

        var user = _loggedUser.GetUser();
        if (user.Role != UserRole.PROVIDER)
            throw new ErrorOnUnauthorizedException("Only providers can access my agenda.");

        var window = (DomainAgendaWindow)(int)request.Window;
        var days = window == DomainAgendaWindow.Week ? 7 : 1;

        var (startUtc, endUtc) = await AgendaDateRangeResolver.ResolveUtcRangeAsync(
            request.Date,
            days,
            request.LocationId,
            _locationReadRepository,
            ct);

        var status = request.Status.HasValue ? (DomainAppointmentStatus?)(int)request.Status.Value : null;
        var items = await _repo.GetAgendaAsync(startUtc, endUtc, request.LocationId, user.Id, status, ct);

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

    private static void ValidateRequest(RequestGetMyAgendaJson request)
    {
        var validator = new GetMyAgendaValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
    }

}
