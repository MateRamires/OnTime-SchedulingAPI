using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security;
using OnTimeScheduling.Application.Validators.Users;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Domain.Entities.User;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.UseCases.Users.CreateUser;

public class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHashService _passwordHashService;
    public CreateUserUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordHashService passwordHashService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHashService = passwordHashService;
    }

    public async Task<Guid> ExecuteAsync(RequestRegisterUserJson request, CancellationToken ct = default)
    {

        await Validate(request, ct);

        //TODO: hash on password before creating new user.

        var user = new User(
            companyId: null, //TODO: Config the getting companyId from User's claims.
            name: request.Name,
            email: request.Email,
            passwordHash: request.PasswordHash,
            role: (UserRole) (int) request.Role
        );

        await _userRepository.Add(user);
        await _unitOfWork.Commit();

        return user.Id;
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

            //TODO: create custom Throw new Error...
        }
    }
}
