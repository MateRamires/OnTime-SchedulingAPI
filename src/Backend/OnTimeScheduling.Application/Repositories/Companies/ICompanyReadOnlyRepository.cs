namespace OnTimeScheduling.Application.Repositories.Companies;

public interface ICompanyReadOnlyRepository
{
    Task<bool> ExistsActiveCompanyWithCNPJ(string cnpj, CancellationToken ct);
}
