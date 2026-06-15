namespace OnTimeScheduling.Application.UseCases.Companies;

public interface IInactivateCompanyUseCase
{
    Task ExecuteAsync(Guid companyId, CancellationToken ct = default);
}
