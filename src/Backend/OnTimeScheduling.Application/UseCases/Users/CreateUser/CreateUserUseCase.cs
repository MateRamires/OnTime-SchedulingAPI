using FluentValidation.Results;
using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Password;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Application.Validators.Users;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Entities.User;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Domain.Extensions;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Users.CreateUser;

public class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ITenantProvider _tenantProvider;
    public CreateUserUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHashService passwordHashService, IAccessTokenGenerator tokenGenerator, ITenantProvider tenantProvider)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHashService = passwordHashService;
        _tenantProvider = tenantProvider;
    }

    public async Task<ResponseRegisteredUserJson> ExecuteAsync(RequestRegisterUserJson request, CancellationToken ct = default)
    {
        request.Email = request.Email.SanitizeEmail();
        request.Name = request.Name.FormatName();

        await Validate(request, ct);
       
        var passwordHash = _passwordHashService.Hash(request.Password);

        var user = new User (
            companyId: _tenantProvider.CompanyId!.Value,
            name: request.Name,
            email: request.Email,
            passwordHash: passwordHash,
            role: (UserRole) (int) request.Role
        );

        await _userRepository.Add(user);
        await _unitOfWork.Commit();

        return new ResponseRegisteredUserJson 
        {
            Name = user.Name
        }; 
    }

    private async Task Validate(RequestRegisterUserJson request, CancellationToken ct = default) 
    {
        var validator = new RegisterUserValidator();

        var result = validator.Validate(request);

        var currentCompanyId = _tenantProvider.CompanyId;
        if (currentCompanyId is null)
            result.Errors.Add(new ValidationFailure(string.Empty, "Only users linked to a company can create new employees."));
        

        if ((UserRole)(int)request.Role == UserRole.SUPER_ADMIN)
            result.Errors.Add(new ValidationFailure(string.Empty, "Operation not allowed for this role."));

        var emailExists = await _userRepository.EmailExists(request.Email, ct);
        if (emailExists)
            result.Errors.Add(new ValidationFailure(string.Empty, "The Email is Already Registered!"));

        if (!result.IsValid) 
        { 
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
