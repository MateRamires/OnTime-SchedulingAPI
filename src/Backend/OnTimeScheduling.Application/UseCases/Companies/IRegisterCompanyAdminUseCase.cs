using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Companies;

public interface IRegisterCompanyAdminUseCase
{
    Task<ResponseRegisteredUserJson> ExecuteAsync(Guid companyId, RequestRegisterCompanyAdminJson request, CancellationToken ct = default);
}
