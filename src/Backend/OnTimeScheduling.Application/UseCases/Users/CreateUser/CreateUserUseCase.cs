using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Password;
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
    private readonly IAccessTokenGenerator _tokenGenerator;
    public CreateUserUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHashService passwordHashService, IAccessTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHashService = passwordHashService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<ResponseRegisteredUserJson> ExecuteAsync(RequestRegisterUserJson request, CancellationToken ct = default)
    {
        request.Email = request.Email.SanitizeEmail();
        request.Name = request.Name.FormatName();

        await Validate(request, ct);

        var passwordHash = _passwordHashService.Hash(request.Password);

        var user = new User (
            companyId: null, //TODO: Config the getting companyId from User's claims.
            name: request.Name,
            email: request.Email,
            passwordHash: passwordHash,
            role: (UserRole) (int) request.Role
        );

        await _userRepository.Add(user);
        await _unitOfWork.Commit();

        var token = _tokenGenerator.Generate(user);

        return new ResponseRegisteredUserJson 
        {
            Name = user.Name,
            AccessToken = token
        }; 
    }

    private async Task Validate(RequestRegisterUserJson request, CancellationToken ct = default) 
    {
        var validator = new RegisterUserValidator();

        var result = validator.Validate(request);

        var emailExists = await _userRepository.EmailExists(request.Email, ct);
        if (emailExists)
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "The Email is Already Registered!"));

        if (!result.IsValid) 
        { 
            var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
