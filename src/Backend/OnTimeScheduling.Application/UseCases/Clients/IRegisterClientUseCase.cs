using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Clients;

public interface IRegisterClientUseCase
{
    Task<ResponseRegisterClientJson> ExecuteAsync(RequestRegisterClientJson request, CancellationToken ct = default);
}
