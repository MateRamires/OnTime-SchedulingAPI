using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Repositories.Companies;
using OnTimeScheduling.Domain.Entities.Company;

namespace OnTimeScheduling.Infrastructure.Persistence.DataAccess.Repositories;

public class CompanyRepository : ICompanyWriteOnlyRepository, ICompanyReadOnlyRepository
{
    private readonly AppDbContext _db;

    public CompanyRepository(AppDbContext dbContext)
    {
        _db = dbContext;
    }
    public async Task Add(Company company, CancellationToken ct = default)
    {
        await _db.Companies.AddAsync(company, ct);
    }

    public async Task<bool> ExistsActiveCompanyWithCNPJ(string cnpj, CancellationToken ct)
    {
        return await _db.Companies
            .AsNoTracking()
            .AnyAsync(c => c.Document.Equals(cnpj) && c.Status == Domain.Enums.RecordStatus.Active, ct);
    }
}
