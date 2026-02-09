using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Companies;

public interface IRegisterCompanyUseCase
{
    public Task<ResponseRegisterCompanyJson> ExecuteAsync(RequestRegisterCompanyJson request, CancellationToken ct = default);
}
