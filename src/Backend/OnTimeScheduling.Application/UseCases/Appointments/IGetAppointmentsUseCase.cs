using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public interface IGetAppointmentsUseCase
{
    Task<ResponsePagedResultJson<ResponseAppointmentSummaryJson>> ExecuteAsync(RequestGetAppointmentsJson request, CancellationToken ct = default);
}
