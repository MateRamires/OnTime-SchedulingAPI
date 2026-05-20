using OnTimeScheduling.Application.Repositories.UnitOfWork;
using OnTimeScheduling.Application.Repositories.Users;
using OnTimeScheduling.Application.Security.Tenant;
using OnTimeScheduling.Application.Security.Token;
using OnTimeScheduling.Exceptions.ExceptionBase;

namespace OnTimeScheduling.Application.UseCases.Users.Management;

public class InactivateUserUseCase : IInactivateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILoggedUser _loggedUser;
    private readonly IUnitOfWork _unitOfWork;

    public InactivateUserUseCase(
        IUserRepository userRepository,
        ITenantProvider tenantProvider,
        ILoggedUser loggedUser,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tenantProvider = tenantProvider;
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid userId, CancellationToken ct = default)
    {
        var loggedUser = _loggedUser.GetUser();
        if (loggedUser.Id == userId)
            throw new ErrorOnValidationException(["Users cannot inactivate their own account."]);

        var companyId = _tenantProvider.CompanyId
            ?? throw new DomainRuleException("It was not possible to identify the company for this user.");

        var user = await _userRepository.GetByIdAndCompanyIncludingInactive(userId, companyId, ct)
            ?? throw new NotFoundException("User not found.");

        user.Inactivate();

        _userRepository.Update(user);
        await _unitOfWork.Commit(ct);
    }

}
