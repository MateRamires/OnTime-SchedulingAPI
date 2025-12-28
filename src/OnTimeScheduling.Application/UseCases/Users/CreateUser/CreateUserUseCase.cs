using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Communication.Requests;
using OnTimeScheduling.Domain.Entities.User;
using OnTimeScheduling.Domain.Enums;

namespace OnTimeScheduling.Application.UseCases.Users.CreateUser;

public class CreateUserUseCase : ICreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateUserUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> ExecuteAsync(RequestRegisterUserJson request, CancellationToken ct = default)
    {

        await Validate(request);

        var user = new User(
            companyId: null,
            name: request.Name,
            email: request.Email,
            password: request.Password,
            role: (UserRole) (int) request.Role
        );

        await _userRepository.Add(user);
        await _unitOfWork.Commit();
    }

    private async Task Validate(RequestRegisterUserJson request) 
    { 
    
    }
}
