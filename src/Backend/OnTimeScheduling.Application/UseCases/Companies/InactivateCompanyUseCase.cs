using OnTimeScheduling.Application.Repositories.Companies;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Companies;

public class InactivateCompanyUseCase : IInactivateCompanyUseCase
{
    private readonly ICompanyReadOnlyRepository _companyReadRepository;
    private readonly ICompanyWriteOnlyRepository _companyWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public InactivateCompanyUseCase(
        ICompanyReadOnlyRepository companyReadRepository,
        ICompanyWriteOnlyRepository companyWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _companyReadRepository = companyReadRepository;
        _companyWriteRepository = companyWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid companyId, CancellationToken ct = default)
    {
        var company = await _companyReadRepository.GetByIdAsync(companyId, ct)
            ?? throw new NotFoundException("Company not found.");

        company.Inactivate();

        _companyWriteRepository.Update(company);
        await _unitOfWork.Commit(ct);
    }
}
