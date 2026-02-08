using OnTimeScheduling.Domain.Entities.Company;

namespace OnTimeScheduling.Application.Repositories.Companies;

public interface ICompanyWriteOnlyRepository
{
    Task Add(Company company, CancellationToken ct = default);
}
