using OnTimeScheduling.Application.Repositories.Companies;
using OnTimeScheduling.Domain.Entities.Company;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;

public class CompanyRepository : ICompanyWriteOnlyRepository
{
    private readonly AppDbContext _dbContext;

    public CompanyRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task Add(Company company, CancellationToken ct)
    {
        await _dbContext.Companies.AddAsync(company, ct);
    }
}
