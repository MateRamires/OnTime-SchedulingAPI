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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IAccessTokenGenerator _tokenGenerator;
    public RegisterCompanyUseCase(ICompanyWriteOnlyRepository companyWriteOnlyRepository, IUserRepository userRepository, 
        IUnitOfWork unitOfWork, IPasswordHashService passwordHashService, IAccessTokenGenerator tokenGenerator)
    {
        _companyRepository = companyWriteOnlyRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHashService = passwordHashService;
        _tokenGenerator = tokenGenerator;
    }
    public async Task<ResponseRegisterCompanyJson> ExecuteAsync(RequestRegisterCompanyJson request, CancellationToken ct = default)
    {
       
        request.Email = request.Email.SanitizeEmail();
        request.Name = request.Name.FormatName();
        // TODO: sanitizar CNPJ/Telefone aqui

        await Validate(request, ct);

        var company = new Domain.Entities.Company.Company(
            request.SocialReason,
            request.FantasyName,
            request.CNPJ,
            request.Phone,
            request.Email // TODO: Criar email especifico da empresa (atualmente pega email do user)
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

        var token = _tokenGenerator.Generate(userAdmin);

        return new ResponseRegisterCompanyJson
        {
            Name = company.FantasyName,
            Token = token 
        };
    }

    private async Task Validate(RequestRegisterCompanyJson request, CancellationToken ct = default)
    {
        var validator = new RegisterCompanyValidator();
        var result = validator.Validate(request);

        var emailExists = await _userRepository.EmailExists(request.Email, ct);
        if (emailExists)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "The Email is Already Registered!"));
        

        // TODO: Validar se CNPJ já existe via Repositório de ReadOnly da Company

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
