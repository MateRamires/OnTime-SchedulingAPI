using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Appointments;

public interface IRegisterAppointmentUseCase
{
    Task<ResponseRegisterAppointmentJson> ExecuteAsync(RequestRegisterAppointmentJson request, CancellationToken ct = default);
}
