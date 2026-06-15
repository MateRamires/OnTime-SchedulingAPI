using OnTimeScheduling.Application.Repositories.Companies;
using OnTimeScheduling.Application.UseCases.Companies.Mapper;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Companies;

public class GetCompanyByIdUseCase : IGetCompanyByIdUseCase
{
    private readonly ICompanyReadOnlyRepository _companyReadRepository;

    public GetCompanyByIdUseCase(ICompanyReadOnlyRepository companyReadRepository)
    {
        _companyReadRepository = companyReadRepository;
    }

    public async Task<ResponseCompanyJson> ExecuteAsync(Guid companyId, CancellationToken ct = default)
    {
        var company = await _companyReadRepository.GetByIdAsync(companyId, ct)
            ?? throw new NotFoundException("Company not found.");

        return CompanyResponseMapper.Map(company);
    }
}
