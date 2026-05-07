using OnTimeScheduling.Application.Repositories.Appointments;
using OnTimeScheduling.Communication.Requests.Appointments;
using OnTimeScheduling.Communication.Responses.Appointments;

namespace OnTimeScheduling.Application.UseCases.Appointments.ReadAgenda;

public class GetDailyAgendaUseCase : IGetDailyAgendaUseCase
{
    private readonly IAppointmentReadOnlyRepository _repo;
    public GetDailyAgendaUseCase(IAppointmentReadOnlyRepository repo) => _repo = repo;

    public async Task<ResponseAgendaJson> ExecuteAsync(RequestGetDailyAgendaJson request, CancellationToken ct = default)
    {
        var startUtc = DateTime.SpecifyKind(request.Date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc);
        var endUtc = startUtc.AddDays(1);

        var items = await _repo.GetAgendaAsync(startUtc, endUtc, request.LocationId, request.ProfessionalId, request.Status, ct);

        return new ResponseAgendaJson
        {
            RangeStartUtc = startUtc,
            RangeEndUtc = endUtc,
            Items = items.Select(i => new ResponseAppointmentAgendaItemJson
            {
                AppointmentId = i.AppointmentId,
                ClientId = i.ClientId,
                ClientName = i.ClientName,
                ProfessionalId = i.ProfessionalId,
                ProfessionalName = i.ProfessionalName,
                LocationId = i.LocationId,
                LocationName = i.LocationName,
                ServiceId = i.ServiceId,
                ServiceName = i.ServiceName,
                Status = i.Status,
                StartTimeUtc = i.StartTimeUtc,
                EndTimeUtc = i.EndTimeUtc
            }).ToList()
        };
    }

}
