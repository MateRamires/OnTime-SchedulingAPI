using OnTimeScheduling.Communication.Requests.Appointments;
using OnTimeScheduling.Communication.Responses.Appointments;

namespace OnTimeScheduling.Application.UseCases.Appointments.ReadAgenda;

public interface IGetDailyAgendaUseCase
{
    Task<ResponseAgendaJson> ExecuteAsync(RequestGetDailyAgendaJson request, CancellationToken ct = default);
}
