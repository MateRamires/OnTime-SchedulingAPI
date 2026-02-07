using OnTimeScheduling.Application.Repositories.Companies;
using OnTimeScheduling.Domain.Entities.Company;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;

public class CompanyRepository : ICompanyWriteOnlyRepository
{
    private readonly AppDbContext _db;

    public CompanyRepository(AppDbContext dbContext)
    {
        _db = dbContext;
    }
    public async Task Add(Company company, CancellationToken ct)
    {
        await _db.Companies.AddAsync(company, ct);
    }
}
