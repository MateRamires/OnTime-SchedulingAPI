using OnTimeScheduling.Communication.Requests.Appointments;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Communication.Responses.Appointments;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public interface IGetAppointmentsUseCase
{
    Task<ResponsePagedResultJson<ResponseAppointmentSummaryJson>> ExecuteAsync(RequestGetAppointmentsJson request, CancellationToken ct = default);
}
