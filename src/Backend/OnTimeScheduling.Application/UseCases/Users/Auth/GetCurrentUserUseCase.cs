using OnTimeScheduling.Application.Repositories.Companies;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Application.UseCases.Users.Auth.Mapper;
using OnTimeScheduling.Communication.Responses;
using OnTimeScheduling.Domain.Enums;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Users.Auth;

public class GetCurrentUserUseCase : IGetCurrentUserUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserRepository _userRepository;
    private readonly ICompanyReadOnlyRepository _companyRepository;

    public GetCurrentUserUseCase(
        ILoggedUser loggedUser,
        IUserRepository userRepository,
        ICompanyReadOnlyRepository companyRepository)
    {
        _loggedUser = loggedUser;
        _userRepository = userRepository;
        _companyRepository = companyRepository;
    }

    public async Task<ResponseUserProfileJson> ExecuteAsync(CancellationToken ct = default)
    {
        var authenticatedUser = _loggedUser.GetUser();
        var user = await _userRepository.GetById(authenticatedUser.Id, ct);

        if (user is null || user.Status != RecordStatus.Active)
            throw new InvalidLoginException("The authenticated session is no longer valid.");

        if (user.CompanyId.HasValue && !await _companyRepository.IsCompanyActive(user.CompanyId.Value, ct))
            throw new InvalidLoginException("The authenticated session is no longer valid.");

        return UserProfileResponseMapper.Map(user);
    }
}
