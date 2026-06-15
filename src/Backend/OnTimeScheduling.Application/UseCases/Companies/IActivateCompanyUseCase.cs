namespace OnTimeScheduling.Application.UseCases.Companies;

public interface IActivateCompanyUseCase
{
    Task ExecuteAsync(Guid companyId, CancellationToken ct = default);
}
