using FluentValidation.Results;
using OnTimeScheduling.Application.Repositories.Companies;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Password;
using OnTimeScheduling.Application.Validators.Companies;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.User;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Domain.Extensions;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Companies;

public class RegisterCompanyAdminUseCase : IRegisterCompanyAdminUseCase
{
    private readonly ICompanyReadOnlyRepository _companyReadRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCompanyAdminUseCase(
        ICompanyReadOnlyRepository companyReadRepository,
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        IUnitOfWork unitOfWork)
    {
        _companyReadRepository = companyReadRepository;
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisteredUserJson> ExecuteAsync(Guid companyId, RequestRegisterCompanyAdminJson request, CancellationToken ct = default)
    {
        request.Email = request.Email.SanitizeEmail();
        request.Name = request.Name.FormatName();

        await Validate(companyId, request, ct);

        var passwordHash = _passwordHashService.Hash(request.Password);
        var userAdmin = new User(
            companyId: companyId,
            name: request.Name,
            email: request.Email,
            passwordHash: passwordHash,
            role: UserRole.COMPANY_ADMIN);

        await _userRepository.Add(userAdmin, ct);
        await _unitOfWork.Commit(ct);

        return new ResponseRegisteredUserJson
        {
            Name = userAdmin.Name
        };
    }

    private async Task Validate(Guid companyId, RequestRegisterCompanyAdminJson request, CancellationToken ct)
    {
        var validator = new RegisterCompanyAdminValidator();
        var result = validator.Validate(request);

        var company = await _companyReadRepository.GetByIdAsync(companyId, ct);
        if (company is null)
            throw new NotFoundException("Company not found.");

        if (company.Status != RecordStatus.Active)
            result.Errors.Add(new ValidationFailure(string.Empty, "Only active companies can receive new company admins."));

        var emailExists = await _userRepository.EmailExists(request.Email, ct);
        if (emailExists)
            result.Errors.Add(new ValidationFailure(nameof(request.Email), "The Email is Already Registered!"));

        if (!result.IsValid)
            throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());
    }
}
