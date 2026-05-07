namespace OnTimeScheduling.Application.UseCases.Appointments.ReadAgenda;

public interface IGetDailyAgendaUseCase
{
    Task<ResponseAgendaJson> ExecuteAsync(RequestGetDailyAgendaJson request, CancellationToken ct = default);
}
