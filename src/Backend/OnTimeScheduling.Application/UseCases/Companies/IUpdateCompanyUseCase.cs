using OnTimeScheduling.Communication.Requests;

namespace OnTimeScheduling.Application.UseCases.Companies;

public interface IUpdateCompanyUseCase
{
    Task ExecuteAsync(Guid companyId, RequestUpdateCompanyJson request, CancellationToken ct = default);
}
