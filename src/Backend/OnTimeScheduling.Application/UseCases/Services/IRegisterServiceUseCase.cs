using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Services;

public interface IRegisterServiceUseCase
{
    public Task<ResponseRegisterServiceJson> ExecuteAsync(RequestRegisterServiceJson request, CancellationToken cancellationToken);
}
