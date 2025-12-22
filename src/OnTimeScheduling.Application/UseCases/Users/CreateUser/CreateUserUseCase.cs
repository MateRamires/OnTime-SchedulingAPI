using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Domain.Entities.User;

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

    public async Task<Guid> ExecuteAsync(User user)
    {
        /*var newUser = new User();

        await _userRepository.AddAsync(newUser);
        await _unitOfWork.CommitAsync();
        
        return newUser.Id;*/
    }
}
