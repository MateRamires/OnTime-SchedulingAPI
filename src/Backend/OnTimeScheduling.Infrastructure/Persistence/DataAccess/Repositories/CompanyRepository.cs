using Microsoft.EntityFrameworkCore;
using OnTimeScheduling.Application.Repositories.Companies;
using OnTimeScheduling.Domain.Entities.Company;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Domain.Extensions;

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

    public void Update(Company company)
    {
        _db.Companies.Update(company);
    }

    public Task<Company?> GetByIdAsync(Guid companyId, CancellationToken ct = default)
    {
        return _db.Companies.FirstOrDefaultAsync(company => company.Id == companyId, ct);
    }

    public async Task<(List<Company> Items, int TotalItems)> GetAllAsync(int skip, int take, RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default)
    {
        var query = _db.Companies.AsNoTracking().AsQueryable();

        if (status.HasValue)
            query = query.Where(company => company.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.Trim().ToLower();
            var numericSearchTerm = searchTerm.RemoveNonNumeric();

            query = query.Where(company =>
                company.SocialReason.ToLower().Contains(normalizedSearchTerm) ||
                company.FantasyName.ToLower().Contains(normalizedSearchTerm) ||
                company.Email.ToLower().Contains(normalizedSearchTerm) ||
                (!string.IsNullOrEmpty(numericSearchTerm) && company.Phone.Contains(numericSearchTerm)) ||
                (!string.IsNullOrEmpty(numericSearchTerm) && company.Document.Contains(numericSearchTerm)));
        }

        var totalItems = await query.CountAsync(ct);

        var items = await query
            .OrderBy(company => company.FantasyName)
            .ThenBy(company => company.SocialReason)
            .ThenBy(company => company.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

        return (items, totalItems);
    }

    public async Task<bool> ExistsActiveCompanyWithCNPJ(string cnpj, CancellationToken ct = default)
    {
        var normalizedCnpj = cnpj.RemoveNonNumeric();

        return await _db.Companies
            .AsNoTracking()
            .AnyAsync(company => company.Document.Equals(normalizedCnpj) && company.Status == RecordStatus.Active, ct);
    }

    public async Task<bool> ExistsCompanyWithCNPJ(string cnpj, CancellationToken ct = default)
    {
        var normalizedCnpj = cnpj.RemoveNonNumeric();

        return await _db.Companies
            .AsNoTracking()
            .AnyAsync(company => company.Document.Equals(normalizedCnpj), ct);
    }

    public async Task<bool> ExistsCompanyWithCNPJExceptId(string cnpj, Guid companyId, CancellationToken ct = default)
    {
        var normalizedCnpj = cnpj.RemoveNonNumeric();

        return await _db.Companies
            .AsNoTracking()
            .AnyAsync(company => company.Document.Equals(normalizedCnpj) && company.Id != companyId, ct);
    }

    public async Task<bool> IsCompanyActive(Guid companyId, CancellationToken ct = default)
    {
        return await _db.Companies
            .AsNoTracking()
            .AnyAsync(company => company.Id == companyId && company.Status == RecordStatus.Active, ct);
    }
}
