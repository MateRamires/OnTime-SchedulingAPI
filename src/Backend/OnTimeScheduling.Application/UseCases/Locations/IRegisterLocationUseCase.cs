using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Locations;

public interface IRegisterLocationUseCase
{
    public Task<ResponseRegisterLocationJson> ExecuteAsync(RequestRegisterLocationJson request, CancellationToken cancellationToken);
}
