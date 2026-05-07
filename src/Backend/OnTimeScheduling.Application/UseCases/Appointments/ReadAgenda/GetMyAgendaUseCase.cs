using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Communication.Requests.Appointments;
using OnTimeScheduling.Communication.Responses.Appointments;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Appointments.ReadAgenda;

public class GetMyAgendaUseCase : IGetMyAgendaUseCase
{
    private readonly IAppointmentReadOnlyRepository _repo;
    private readonly ILoggedUser _loggedUser;

    public GetMyAgendaUseCase(IAppointmentReadOnlyRepository repo, ILoggedUser loggedUser)
    {
        _repo = repo;
        _loggedUser = loggedUser;
    }

    public async Task<ResponseAgendaJson> ExecuteAsync(RequestGetMyAgendaJson request, CancellationToken ct = default)
    {
        var user = _loggedUser.GetUser();
        if (user.Role != UserRole.PROVIDER)
            throw new ErrorOnUnauthorizedException("Only providers can access my agenda.");

        var startUtc = DateTime.SpecifyKind(request.Date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var endUtc = request.Window == AgendaWindow.Week ? startUtc.AddDays(7) : startUtc.AddDays(1);

        var items = await _repo.GetAgendaAsync(startUtc, endUtc, request.LocationId, user.Id, request.Status, ct);

        return new ResponseAgendaJson { RangeStartUtc = startUtc, RangeEndUtc = endUtc, Items = items.Select(i => new ResponseAppointmentAgendaItemJson { AppointmentId = i.AppointmentId, ClientId = i.ClientId, ClientName = i.ClientName, ProfessionalId = i.ProfessionalId, ProfessionalName = i.ProfessionalName, LocationId = i.LocationId, LocationName = i.LocationName, ServiceId = i.ServiceId, ServiceName = i.ServiceName, Status = i.Status, StartTimeUtc = i.StartTimeUtc, EndTimeUtc = i.EndTimeUtc }).ToList() };
    }

}
