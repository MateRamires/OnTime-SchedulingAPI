using OnTimeScheduling.Communication.Responses;

namespace OnTimeScheduling.Application.UseCases.Companies;

public interface IGetCompanyByIdUseCase
{
    Task<ResponseCompanyJson> ExecuteAsync(Guid companyId, CancellationToken ct = default);
}
