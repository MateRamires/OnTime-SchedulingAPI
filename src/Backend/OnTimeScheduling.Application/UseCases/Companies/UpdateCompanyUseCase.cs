using FluentValidation.Results;
using OnTimeScheduling.Application.Repositories.Companies;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Validators.Companies;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Domain.Extensions;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Companies;

public class UpdateCompanyUseCase : IUpdateCompanyUseCase
{
    private readonly ICompanyReadOnlyRepository _companyReadRepository;
    private readonly ICompanyWriteOnlyRepository _companyWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompanyUseCase(
        ICompanyReadOnlyRepository companyReadRepository,
        ICompanyWriteOnlyRepository companyWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _companyReadRepository = companyReadRepository;
        _companyWriteRepository = companyWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid companyId, RequestUpdateCompanyJson request, CancellationToken ct = default)
    {
        request.CompanyEmail = request.CompanyEmail.SanitizeEmail();
        request.CNPJ = request.CNPJ.RemoveNonNumeric();
        request.Phone = request.Phone.RemoveNonNumeric();
        request.SocialReason = request.SocialReason.Trim();
        request.FantasyName = request.FantasyName.Trim();

        var company = await _companyReadRepository.GetByIdAsync(companyId, ct)
            ?? throw new NotFoundException("Company not found.");

        await Validate(companyId, request, ct);

        company.Update(
            request.SocialReason,
            request.FantasyName,
            request.CNPJ,
            request.Phone,
            request.CompanyEmail);

        _companyWriteRepository.Update(company);
        await _unitOfWork.Commit(ct);
    }

    private async Task Validate(Guid companyId, RequestUpdateCompanyJson request, CancellationToken ct)
    {
        var validator = new UpdateCompanyValidator();
        var result = validator.Validate(request);

        var cnpjExists = await _companyReadRepository.ExistsCompanyWithCNPJExceptId(request.CNPJ, companyId, ct);
        if (cnpjExists)
            result.Errors.Add(new ValidationFailure(nameof(request.CNPJ), "Company with this CNPJ is already registered!"));

        if (!result.IsValid)
            throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());
    }
}
