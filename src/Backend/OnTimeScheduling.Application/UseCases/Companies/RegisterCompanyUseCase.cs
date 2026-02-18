using OnTimeScheduling.Application.Repositories.Companies;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Password;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Application.Validators.Companies;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.User;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Domain.Extensions;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Companies;

public class RegisterCompanyUseCase : IRegisterCompanyUseCase
{
    private readonly ICompanyWriteOnlyRepository _companyRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICompanyReadOnlyRepository _companyReadRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHashService _passwordHashService;
    public RegisterCompanyUseCase(ICompanyWriteOnlyRepository companyWriteOnlyRepository, IUserRepository userRepository, 
        IUnitOfWork unitOfWork, IPasswordHashService passwordHashService, ICompanyReadOnlyRepository companyReadRepository)
    {
        _companyRepository = companyWriteOnlyRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHashService = passwordHashService;
        _companyReadRepository = companyReadRepository;
    }
    public async Task<ResponseRegisterCompanyJson> ExecuteAsync(RequestRegisterCompanyJson request, CancellationToken ct = default)
    {
       
        request.Email = request.Email.SanitizeEmail();
        request.CompanyEmail = request.CompanyEmail.SanitizeEmail();
        request.Name = request.Name.FormatName();
        request.CNPJ = request.CNPJ.RemoveNonNumeric(); 
        request.Phone = request.Phone.RemoveNonNumeric();

        await Validate(request, ct);

        var company = new Domain.Entities.Company.Company(
            request.SocialReason,
            request.FantasyName,
            request.CNPJ,
            request.Phone,
            request.CompanyEmail
        );

        var passwordHash = _passwordHashService.Hash(request.Password);

        var userAdmin = new User(
            companyId: company.Id,
            name: request.Name,
            email: request.Email, 
            passwordHash: passwordHash,
            role: UserRole.COMPANY_ADMIN 
        );

        await _companyRepository.Add(company, ct);
        await _userRepository.Add(userAdmin, ct);

        await _unitOfWork.Commit();


        return new ResponseRegisterCompanyJson
        {
            Name = company.FantasyName
        };
    }

    private async Task Validate(RequestRegisterCompanyJson request, CancellationToken ct = default)
    {
        var validator = new RegisterCompanyValidator();
        var result = validator.Validate(request);

        var emailExists = await _userRepository.EmailExists(request.Email, ct);
        if (emailExists)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "The Email is Already Registered!"));

        var cnpjExists = await _companyReadRepository.ExistsActiveCompanyWithCNPJ(request.CNPJ, ct);
        if (cnpjExists)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "Company with this CNPJ is already registered!"));

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
