using OnTimeScheduling.Domain.Entities.Company;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.Repositories.Companies;

public interface ICompanyReadOnlyRepository
{
    Task<Company?> GetByIdAsync(Guid companyId, CancellationToken ct = default);
    Task<(List<Company> Items, int TotalItems)> GetAllAsync(int skip, int take, RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default);
    Task<bool> ExistsActiveCompanyWithCNPJ(string cnpj, CancellationToken ct = default);
    Task<bool> ExistsCompanyWithCNPJ(string cnpj, CancellationToken ct = default);
    Task<bool> ExistsCompanyWithCNPJExceptId(string cnpj, Guid companyId, CancellationToken ct = default);
    Task<bool> IsCompanyActive(Guid companyId, CancellationToken ct = default);
}
