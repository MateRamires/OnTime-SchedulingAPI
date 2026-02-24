using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Services;

public interface ILinkProfessionalServiceUseCase
{
    public Task<ResponseLinkProfessionalServiceJson> ExecuteAsync(RequestLinkProfessionalServiceJson request, CancellationToken ct = default);
}
