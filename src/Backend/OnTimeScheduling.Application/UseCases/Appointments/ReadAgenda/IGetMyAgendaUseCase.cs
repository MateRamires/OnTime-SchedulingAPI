namespace OnTimeScheduling.Application.UseCases.Appointments.ReadAgenda;

public interface IGetMyAgendaUseCase
{
    Task<ResponseAgendaJson> ExecuteAsync(RequestGetMyAgendaJson request, CancellationToken ct = default);
}
