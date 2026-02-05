using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Companies;

public class RegisterCompanyUseCase : IRegisterCompanyUseCase
{
    public Task<ResponseRegisterCompanyJson> ExecuteAsync(RequestRegisterCompanyJson request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
