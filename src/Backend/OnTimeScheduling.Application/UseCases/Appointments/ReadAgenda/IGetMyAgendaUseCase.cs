using OnTimeScheduling.Communication.Requests.Appointments;
using OnTimeScheduling.Communication.Responses.Appointments;

namespace OnTimeScheduling.Application.UseCases.Appointments.ReadAgenda;

public interface IGetMyAgendaUseCase
{
    Task<ResponseAgendaJson> ExecuteAsync(RequestGetMyAgendaJson request, CancellationToken ct = default);
}
