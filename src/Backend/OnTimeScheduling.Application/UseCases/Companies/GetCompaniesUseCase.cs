using OnTimeScheduling.Application.Repositories.Companies;
using OnTimeScheduling.Application.UseCases.Companies.Mapper;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.UseCases.Companies;

public class GetCompaniesUseCase : IGetCompaniesUseCase
{
    private readonly ICompanyReadOnlyRepository _companyReadRepository;

    public GetCompaniesUseCase(ICompanyReadOnlyRepository companyReadRepository)
    {
        _companyReadRepository = companyReadRepository;
    }

    public async Task<ResponsePagedResultJson<ResponseCompanyJson>> ExecuteAsync(RequestPaginationQuery pagination, RecordStatus? status = null, string? searchTerm = null, CancellationToken ct = default)
    {
        var (companies, totalItems) = await _companyReadRepository.GetAllAsync(pagination.Skip, pagination.Size, status, searchTerm, ct);
        var items = companies.Select(CompanyResponseMapper.Map).ToList();

        return new ResponsePagedResultJson<ResponseCompanyJson>
        {
            Page = pagination.Page,
            Size = pagination.Size,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pagination.Size),
            Items = items
        };
    }
}
